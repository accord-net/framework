// Accord.NET Sample Applications
// http://accord-framework.net
//
// Copyright © 2009-2014, César Souza
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Accord.Math;
using Accord.Neuro;
using Accord.Neuro.Learning;
using Accord.Statistics.Analysis;
using Accord.Statistics.Formats;
using AForge.Neuro;
using ZedGraph;

namespace Classification.ANNs
{
    public partial class MainForm : Form
    {

        private ActivationNetwork ann;

        string[] sourceColumns;
        double[,] sourceMatrix;

        private double learningRate = 0.1;
        private double sigmoidAlphaValue = 2.0;
        private int neuronsInFirstLayer = 10;
        private int iterations = 50;
        private bool useRegularization;
        private bool useNguyenWidrow;
        private bool useSameWeights;

        private Thread workerThread = null;
        private volatile bool needToStop = false;


        public MainForm()
        {
            InitializeComponent();

            dgvLearningSource.AutoGenerateColumns = true;
            dgvPerformance.AutoGenerateColumns = false;

            openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Resources");

            UpdateSettings();
        }




        private void MenuFileOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string filename = openFileDialog.FileName;
                string extension = Path.GetExtension(filename);
                if (extension == ".xls" || extension == ".xlsx")
                {
                    ExcelReader db = new ExcelReader(filename, true, false);
                    TableSelectDialog t = new TableSelectDialog(db.GetWorksheetList());

                    if (t.ShowDialog(this) == DialogResult.OK)
                    {
                        DataTable tableSource = db.GetWorksheet(t.Selection);

                        double[,] sourceMatrix = tableSource.ToMatrix(out sourceColumns);

                        // Detect the kind of problem loaded.
                        if (sourceMatrix.GetLength(1) == 2)
                        {
                            MessageBox.Show("Missing class column.");
                        }
                        else
                        {
                            this.dgvLearningSource.DataSource = tableSource;
                            this.dgvTestingSource.DataSource = tableSource.Copy();


                            CreateScatterplot(graphInput, sourceMatrix);

                            // enable "Start" button
                            startButton.Enabled = true;
                        }
                    }
                }
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

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (dgvLearningSource.DataSource == null)
            {
                MessageBox.Show("Please load some data first.");
                return;
            }

            // Finishes and save any pending changes to the given data
            dgvLearningSource.EndEdit();

            // Creates a matrix from the source data table
            sourceMatrix = (dgvLearningSource.DataSource as DataTable).ToMatrix(out sourceColumns);


            // get learning rate
            try { learningRate = double.Parse(learningRateBox.Text); }
            catch { learningRate = 0.1; }

            // get sigmoid's alpha value
            try { sigmoidAlphaValue = Math.Max(0.001, Math.Min(50, double.Parse(alphaBox.Text))); }
            catch { sigmoidAlphaValue = 2; }

            // get neurons count in first layer
            try { neuronsInFirstLayer = Math.Max(5, Math.Min(1000, int.Parse(neuronsBox.Text))); }
            catch { neuronsInFirstLayer = 20; }

            // iterations
            try { iterations = Math.Max(0, int.Parse(iterationsBox.Text)); }
            catch { iterations = 100; }

            useRegularization = cbRegularization.Checked;
            useNguyenWidrow = cbNguyenWidrow.Checked;
            useSameWeights = cbSameWeights.Checked;

            // update settings controls
            UpdateSettings();

            // disable all settings controls except "Stop" button
            EnableControls(false);

            // run worker thread
            needToStop = false;
            workerThread = new Thread(new ThreadStart(SearchSolution));
            workerThread.Start();
        }

        // Delegates to enable async calls for setting controls properties
        private delegate void EnableCallback(bool enable);

        // Enable/disable controls (safe for threading)
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
                neuronsBox.Enabled = enable;
                iterationsBox.Enabled = enable;

                startButton.Enabled = enable;
                stopButton.Enabled = !enable;
            }
        }


        int iteration;
        double error;

        // Worker thread
        void SearchSolution()
        {
            // number of learning samples
            int samples = sourceMatrix.GetLength(0);

            // prepare learning data
            double[][] inputs = sourceMatrix.Submatrix(null, 0, 1).ToArray();
            double[][] outputs = sourceMatrix.GetColumn(2).Transpose().ToArray();

            // create multi-layer neural network
            ann = new ActivationNetwork(
                new BipolarSigmoidFunction(sigmoidAlphaValue),
                2, neuronsInFirstLayer, 1);

            if (useNguyenWidrow)
            {
                if (useSameWeights)
                    Accord.Math.Tools.SetupGenerator(0);

                NguyenWidrow initializer = new NguyenWidrow(ann);
                initializer.Randomize();
            }

            // create teacher
            LevenbergMarquardtLearning teacher = new LevenbergMarquardtLearning(ann, useRegularization);

            // set learning rate and momentum
            teacher.LearningRate = learningRate;

            // iterations
            iteration = 1;

            var ranges = Matrix.Range(sourceMatrix, 0);
            double[][] map = Matrix.Mesh(ranges[0], ranges[1], 0.05, 0.05);
            var sw = Stopwatch.StartNew();

            // loop
            while (!needToStop)
            {
                // run epoch of learning procedure
                error = teacher.RunEpoch(inputs, outputs) / samples;

                var result = map.Apply(ann.Compute).GetColumn(0).Apply(Math.Sign);

                var graph = map.ToMatrix().InsertColumn(result.ToDouble());

                CreateScatterplot(zedGraphControl2, graph);

                // increase current iteration
                iteration++;

                elapsed = sw.Elapsed;

                updateStatus();

                // check if we need to stop
                if ((iterations != 0) && (iteration > iterations))
                    break;
            }

            sw.Stop();

            // enable settings controls
            EnableControls(true);
        }

        TimeSpan elapsed = TimeSpan.Zero;

        private void updateStatus()
        {
            if (!currentIterationBox.InvokeRequired)
            {
                currentIterationBox.Text = iteration.ToString();
                currentErrorBox.Text = error.ToString("N10");
                currentElapsed.Text = elapsed.ToString();
            }
            else
            {
                currentIterationBox.BeginInvoke(new Action(updateStatus));
            }
        }

        private void btnTestingRun_Click(object sender, EventArgs e)
        {
            if (ann == null || dgvTestingSource.DataSource == null)
            {
                MessageBox.Show("Please create a machine first.");
                return;
            }


            // Creates a matrix from the source data table
            double[,] sourceMatrix = (dgvTestingSource.DataSource as DataTable).ToMatrix();


            // Extract inputs
            double[][] inputs = new double[sourceMatrix.GetLength(0)][];
            for (int i = 0; i < inputs.Length; i++)
                inputs[i] = new double[] { sourceMatrix[i, 0], sourceMatrix[i, 1] };

            // Get only the label outputs
            int[] expected = new int[sourceMatrix.GetLength(0)];
            for (int i = 0; i < expected.Length; i++)
                expected[i] = (int)sourceMatrix[i, 2];

            // Compute the machine outputs
            int[] output = new int[expected.Length];
            for (int i = 0; i < expected.Length; i++)
                output[i] = System.Math.Sign(ann.Compute(inputs[i])[0]);

            double[] expectedd = new double[expected.Length];
            double[] outputd = new double[expected.Length];
            for (int i = 0; i < expected.Length; i++)
            {
                expectedd[i] = expected[i];
                outputd[i] = output[i];
            }

            // Use confusion matrix to compute some statistics.
            ConfusionMatrix confusionMatrix = new ConfusionMatrix(output, expected, 1, -1);
            dgvPerformance.DataSource = new List<ConfusionMatrix> { confusionMatrix };

            foreach (DataGridViewColumn col in dgvPerformance.Columns) col.Visible = true;
            Column1.Visible = Column2.Visible = false;

            // Create performance scatterplot
            CreateResultScatterplot(zedGraphControl1, inputs, expectedd, outputd);
        }



        public void CreateScatterplot(ZedGraphControl zgc, double[,] graph)
        {
            GraphPane myPane = zgc.GraphPane;
            myPane.CurveList.Clear();

            // Set the titles
            myPane.Title.IsVisible = false;
            myPane.XAxis.Title.Text = sourceColumns[0];
            myPane.YAxis.Title.Text = sourceColumns[1];


            // Classification problem
            PointPairList list1 = new PointPairList(); // Z = -1
            PointPairList list2 = new PointPairList(); // Z = +1
            for (int i = 0; i < graph.GetLength(0); i++)
            {
                if (graph[i, 2] == -1)
                    list1.Add(graph[i, 0], graph[i, 1]);
                if (graph[i, 2] == 1)
                    list2.Add(graph[i, 0], graph[i, 1]);
            }

            // Add the curve
            LineItem myCurve = myPane.AddCurve("G1", list1, Color.Blue, SymbolType.Square);
            myCurve.Line.IsVisible = false;
            myCurve.Symbol.Border.IsVisible = false;
            myCurve.Symbol.Fill = new Fill(Color.Blue);

            myCurve = myPane.AddCurve("G2", list2, Color.Green, SymbolType.Square);
            myCurve.Line.IsVisible = false;
            myCurve.Symbol.Border.IsVisible = false;
            myCurve.Symbol.Fill = new Fill(Color.Green);


            // Fill the background of the chart rect and pane
            myPane.Fill = new Fill(Color.WhiteSmoke);

            zgc.AxisChange();
            zgc.Invalidate();
        }


        public void CreateResultScatterplot(ZedGraphControl zgc, double[][] inputs, double[] expected, double[] output)
        {
            GraphPane myPane = zgc.GraphPane;
            myPane.CurveList.Clear();

            // Set the titles
            myPane.Title.IsVisible = false;
            myPane.XAxis.Title.Text = sourceColumns[0];
            myPane.YAxis.Title.Text = sourceColumns[1];



            // Classification problem
            PointPairList list1 = new PointPairList(); // Z = -1, OK
            PointPairList list2 = new PointPairList(); // Z = +1, OK
            PointPairList list3 = new PointPairList(); // Z = -1, Error
            PointPairList list4 = new PointPairList(); // Z = +1, Error
            for (int i = 0; i < output.Length; i++)
            {
                if (output[i] == -1)
                {
                    if (expected[i] == -1)
                        list1.Add(inputs[i][0], inputs[i][1]);
                    if (expected[i] == 1)
                        list3.Add(inputs[i][0], inputs[i][1]);
                }
                else
                {
                    if (expected[i] == -1)
                        list4.Add(inputs[i][0], inputs[i][1]);
                    if (expected[i] == 1)
                        list2.Add(inputs[i][0], inputs[i][1]);
                }
            }

            // Add the curve
            LineItem
            myCurve = myPane.AddCurve("G1 Hits", list1, Color.Blue, SymbolType.Diamond);
            myCurve.Line.IsVisible = false;
            myCurve.Symbol.Border.IsVisible = false;
            myCurve.Symbol.Fill = new Fill(Color.Blue);

            myCurve = myPane.AddCurve("G2 Hits", list2, Color.Green, SymbolType.Diamond);
            myCurve.Line.IsVisible = false;
            myCurve.Symbol.Border.IsVisible = false;
            myCurve.Symbol.Fill = new Fill(Color.Green);

            myCurve = myPane.AddCurve("G1 Miss", list3, Color.Blue, SymbolType.Plus);
            myCurve.Line.IsVisible = false;
            myCurve.Symbol.Border.IsVisible = true;
            myCurve.Symbol.Fill = new Fill(Color.Blue);

            myCurve = myPane.AddCurve("G2 Miss", list4, Color.Green, SymbolType.Plus);
            myCurve.Line.IsVisible = false;
            myCurve.Symbol.Border.IsVisible = true;
            myCurve.Symbol.Fill = new Fill(Color.Green);


            // Fill the background of the chart rect and pane
            myPane.Fill = new Fill(Color.WhiteSmoke);

            zgc.AxisChange();
            zgc.Invalidate();
        }


        private static double computeError(double[][] inputs, double[][] outputs, ActivationNetwork ann)
        {
            // Compute the machine outputs
            int miss = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                var y = System.Math.Sign(ann.Compute(inputs[i])[0]);
                var o = outputs[i][0];
                if (y != o) miss++;
            }

            return (double)miss / inputs.Length;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            needToStop = true;
        }

        private void cbNguyenWidrow_CheckedChanged(object sender, EventArgs e)
        {
            cbSameWeights.Enabled = cbNguyenWidrow.Checked;
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }
    }
}
