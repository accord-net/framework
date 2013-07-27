// Accord.NET Sample Applications
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Kernels;
using ZedGraph;

namespace Handwriting
{
    public partial class MainForm : Form
    {
        // Colors used in the pie graphics.
        private readonly Color[] colors = { Color.YellowGreen, Color.DarkOliveGreen, Color.DarkKhaki, Color.Olive,
            Color.Honeydew, Color.PaleGoldenrod, Color.Indigo, Color.Olive, Color.SeaGreen };


        /*
         * Good parameter choices are:
         *  
         *  Polynomial kernel (degree = 2, constant = 0)
         *  complexity = 1.0
         *  tolerance  = 0.2
         *  Accuracy: 95% (11mb)
         * 
         *  Gaussian kernel (sigma = 6.22)
         *  complexity = 1.5
         *  tolerance  = 0.2
         *  Accuracy: 94% (35mb)
         */

        MulticlassSupportVectorMachine ksvm;


        public MainForm()
        {
            InitializeComponent();
        }


        #region Data extraction
        private Bitmap Extract(string text)
        {
            Bitmap bitmap = new Bitmap(32, 32, PixelFormat.Format32bppRgb);
            string[] lines = text.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    if (lines[i][j] == '0')
                        bitmap.SetPixel(j, i, Color.White);
                    else bitmap.SetPixel(j, i, Color.Black);
                }
            }
            return bitmap;
        }

        private double[] Extract(Bitmap bmp)
        {
            double[] features = new double[32 * 32];
            for (int i = 0; i < 32; i++)
                for (int j = 0; j < 32; j++)
                    features[i * 32 + j] = (bmp.GetPixel(j, i).R == 255) ? 0 : 1;

            return features;
        }

        private Bitmap Export(double[] features)
        {
            Bitmap bitmap = new Bitmap(32, 32, PixelFormat.Format32bppRgb);

            for (int i = 0; i < 32; i++)
                for (int j = 0; j < 32; j++)
                {
                    double v = features[i * 32 + j];
                    v = 255 - Math.Max(0, Math.Min(255, Math.Abs(v) * 255));
                    bitmap.SetPixel(j, i, Color.FromArgb((int)v, (int)v, (int)v));
                }

            return bitmap;
        }

        private double[] Preprocess(Bitmap bitmap)
        {
            double[] features = new double[64];

            for (int m = 0; m < 8; m++)
            {
                for (int n = 0; n < 8; n++)
                {
                    int c = m * 8 + n;
                    for (int i = m * 4; i < m * 4 + 4; i++)
                    {
                        for (int j = n * 4; j < n * 4 + 4; j++)
                        {
                            Color pixel = bitmap.GetPixel(j, i);
                            if (pixel.R == 0x00) // white
                                features[c] += 1;
                        }
                    }
                }
            }

            return features;
        }
        #endregion


        #region Form Events
        private void MainForm_Load(object sender, EventArgs e)
        {
            lbStatus.Text = "Click File->Open to load data.";

            cbStrategy.DataSource = Enum.GetValues(typeof(SelectionStrategy));
        }

        private void btnRunTraining_Click(object sender, EventArgs e)
        {
            if (dgvTrainingSource.Rows.Count == 0)
            {
                MessageBox.Show("Please load the training data before clicking this button");
                return;
            }

            lbStatus.Text = "Gathering data. This may take a while...";
            Application.DoEvents();



            // Extract inputs and outputs
            int rows = dgvTrainingSource.Rows.Count;
            double[][] input = new double[rows][];
            int[] output = new int[rows];
            for (int i = 0; i < rows; i++)
            {
                input[i] = (double[])dgvTrainingSource.Rows[i].Cells["colTrainingFeatures"].Value;
                output[i] = (int)dgvTrainingSource.Rows[i].Cells["colTrainingLabel"].Value;
            }

            // Create the chosen Kernel with given parameters
            IKernel kernel;
            if (rbGaussian.Checked)
            {
                kernel = new Gaussian((double)numSigma.Value);
            }
            else
            {
                if (numDegree.Value == 1)
                    kernel = new Linear((double)numConstant.Value);
                else
                    kernel = new Polynomial((int)numDegree.Value, (double)numConstant.Value);
            }

            // Create the Multi-class Support Vector Machine using the selected Kernel
            ksvm = new MulticlassSupportVectorMachine(1024, kernel, 10);

            // Create the learning algorithm using the machine and the training data
            MulticlassSupportVectorLearning ml = new MulticlassSupportVectorLearning(ksvm, input, output);

            // Extract training parameters from the interface
            double complexity = (double)numComplexity.Value;
            double tolerance = (double)numTolerance.Value;
            int cacheSize = (int)numCache.Value;
            SelectionStrategy strategy = (SelectionStrategy)cbStrategy.SelectedItem;

            // Configure the learning algorithm
            ml.Algorithm = (svm, classInputs, classOutputs, i, j) =>
            {
                var smo = new SequentialMinimalOptimization(svm, classInputs, classOutputs);
                smo.Complexity = complexity;
                smo.Tolerance = tolerance;
                smo.CacheSize = cacheSize;
                smo.Strategy = strategy;
                if (kernel is Linear) smo.Compact = true;
                return smo;
            };


            lbStatus.Text = "Training the classifiers. This may take a (very) significant amount of time...";
            Application.DoEvents();

            Stopwatch sw = Stopwatch.StartNew();

            // Train the machines. It should take a while.
            double error = ml.Run();

            sw.Stop();

            lbStatus.Text = String.Format(
                "Training complete ({0}ms, {1}er). Click Classify to test the classifiers.",
                sw.ElapsedMilliseconds, error);

            btnClassifyVoting.Enabled = true;
            btnClassifyElimination.Enabled = true;
            btnCalibration.Enabled = true;


            // Populate the information tab with the machines
            dgvMachines.Rows.Clear();
            int k = 1;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < i; j++, k++)
                {
                    var machine = ksvm[i, j];

                    int sv = machine.SupportVectors == null ? 0 : machine.SupportVectors.Length;

                    int c = dgvMachines.Rows.Add(k, i + "-vs-" + j, sv, machine.Threshold);
                    dgvMachines.Rows[c].Tag = machine;
                }
            }

            // approximate size in bytes = 
            //   number of support vectors *
            //   number of doubles in a support vector *
            //   size of double
            int bytes = ksvm.SupportVectorUniqueCount * 1024 * sizeof(double);
            float megabytes = bytes / (1024 * 1024);
            lbSize.Text = String.Format("{0} ({1} MB)", ksvm.SupportVectorUniqueCount, megabytes);
        }


        private void btnRunCalibration_Click(object sender, EventArgs e)
        {
            if (ksvm == null)
            {
                MessageBox.Show("Please train the machines first.");
                return;
            }

            // Extract inputs and outputs
            int rows = dgvTrainingSource.Rows.Count;
            double[][] input = new double[rows][];
            int[] output = new int[rows];
            for (int i = 0; i < rows; i++)
            {
                input[i] = (double[])dgvTrainingSource.Rows[i].Cells["colTrainingFeatures"].Value;
                output[i] = (int)dgvTrainingSource.Rows[i].Cells["colTrainingLabel"].Value;
            }

            // Create the learning algorithm using the machine and the training data
            MulticlassSupportVectorLearning ml = new MulticlassSupportVectorLearning(ksvm, input, output);

            // Configure the learning algorithm
            ml.Algorithm = (svm, classInputs, classOutputs, i, j) =>
            {
                return new ProbabilisticOutputLearning(svm, classInputs, classOutputs);
            };


            lbStatus.Text = "Calibrating the classifiers. This may take a (very) significant amount of time...";
            Application.DoEvents();

            Stopwatch sw = Stopwatch.StartNew();

            // Train the machines. It should take a while.
            double error = ml.Run();

            sw.Stop();

            lbStatus.Text = String.Format(
                "Calibration complete ({0}ms, {1}er). Click Classify to test the classifiers.",
                sw.ElapsedMilliseconds, error);

            btnClassifyVoting.Enabled = true;
        }

        private void btnClassify_Click(object sender, EventArgs e)
        {
            if (dgvAnalysisTesting.Rows.Count == 0)
            {
                MessageBox.Show("Please load the testing data before clicking this button");
                return;
            }
            else if (ksvm == null)
            {
                MessageBox.Show("Please perform the training before attempting classification");
                return;
            }

            lbStatus.Text = "Classification started. This may take a while...";
            Application.DoEvents();

            int hits = 0;
            progressBar.Visible = true;
            progressBar.Value = 0;
            progressBar.Step = 1;
            progressBar.Maximum = dgvAnalysisTesting.Rows.Count;

            // Extract inputs
            foreach (DataGridViewRow row in dgvAnalysisTesting.Rows)
            {
                double[] input = (double[])row.Cells["colTestingFeatures"].Value;
                int expected = (int)row.Cells["colTestingExpected"].Value;

                int output;
                if (sender == btnClassifyElimination)
                    output = ksvm.Compute(input, MulticlassComputeMethod.Elimination);
                else
                    output = ksvm.Compute(input, MulticlassComputeMethod.Voting);

                row.Cells["colTestingOutput"].Value = output;

                if (expected == output)
                {
                    row.Cells[0].Style.BackColor = Color.LightGreen;
                    row.Cells[1].Style.BackColor = Color.LightGreen;
                    row.Cells[2].Style.BackColor = Color.LightGreen;
                    hits++;
                }
                else
                {
                    row.Cells[0].Style.BackColor = Color.White;
                    row.Cells[1].Style.BackColor = Color.White;
                    row.Cells[2].Style.BackColor = Color.White;
                }

                progressBar.PerformStep();
            }

            progressBar.Visible = false;

            lbStatus.Text = String.Format("Classification complete. Hits: {0}/{1} ({2:0%})",
                hits, dgvAnalysisTesting.Rows.Count, (double)hits / dgvAnalysisTesting.Rows.Count);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            lbStatus.Text = "Loading data. This may take a while...";
            Application.DoEvents();


            // Load optdigits dataset into the DataGridView
            StringReader reader = new StringReader(Properties.Resources.optdigits_tra);

            int trainingStart = 0;
            int trainingCount = 1000;

            int testingStart = 1000;
            int testingCount = 1000;

            dgvTrainingSource.Rows.Clear();
            dgvAnalysisTesting.Rows.Clear();

            int c = 0;
            int trainingSet = 0;
            int testingSet = 0;
            while (true)
            {
                char[] buffer = new char[(32 + 2) * 32];
                int read = reader.ReadBlock(buffer, 0, buffer.Length);
                string label = reader.ReadLine();


                if (read < buffer.Length || label == null) break;

                if (c > trainingStart && c <= trainingStart + trainingCount)
                {
                    Bitmap bitmap = Extract(new String(buffer));
                    double[] features = Extract(bitmap);
                    int clabel = Int32.Parse(label);
                    dgvTrainingSource.Rows.Add(bitmap, clabel, features);
                    trainingSet++;
                }
                else if (c > testingStart && c <= testingStart + testingCount)
                {
                    Bitmap bitmap = Extract(new String(buffer));
                    double[] features = Extract(bitmap);
                    int clabel = Int32.Parse(label);
                    dgvAnalysisTesting.Rows.Add(bitmap, clabel, null, features);
                    testingSet++;
                }

                c++;
            }

            lbStatus.Text = String.Format(
                "Dataset loaded (training: {0} / testing: {1}). Click Run training to start the training.",
                trainingSet, testingSet);

            btnSampleRunAnalysis.Enabled = true;
        }

        private void btnCanvasClassify_Click(object sender, EventArgs e)
        {
            if (ksvm != null)
            {
                // Get the input vector drawn
                double[] input = canvas.GetDigit();

                // Classify the input vector
                double[] responses;
                int num = ksvm.Compute(input, out responses);

                if (!ksvm.IsProbabilistic)
                {
                    // Normalize responses
                    double max = responses.Max();
                    double min = responses.Min();

                    responses = Accord.Math.Tools.Scale(min, max, 0, 1, responses);
                }

                // Set the actual classification answer 
                lbCanvasClassification.Text = num.ToString();

                // Create the bar graph to show the relative responses
                CreateBarGraph(graphClassification, responses);
            }
        }

        private void btnCanvasClear_Click(object sender, EventArgs e)
        {
            canvas.Clear();
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            btnCanvasClassify_Click(this, EventArgs.Empty);
        }

        private void tbPenWidth_Scroll(object sender, EventArgs e)
        {
            canvas.PenSize = tbPenWidth.Value;
        }
        #endregion


        #region ZedGraph Creation

        public void CreateBarGraph(ZedGraphControl zgc, double[] discriminants)
        {
            GraphPane myPane = zgc.GraphPane;

            myPane.CurveList.Clear();

            myPane.Title.IsVisible = false;
            myPane.Legend.IsVisible = false;
            myPane.Border.IsVisible = false;
            myPane.Border.IsVisible = false;
            myPane.Margin.Bottom = 20f;
            myPane.Margin.Right = 20f;
            myPane.Margin.Left = 20f;
            myPane.Margin.Top = 30f;

            myPane.YAxis.Title.IsVisible = true;
            myPane.YAxis.IsVisible = true;
            myPane.YAxis.MinorGrid.IsVisible = false;
            myPane.YAxis.MajorGrid.IsVisible = false;
            myPane.YAxis.IsAxisSegmentVisible = false;
            myPane.YAxis.Scale.Max = 9.5;
            myPane.YAxis.Scale.Min = -0.5;
            myPane.YAxis.MajorGrid.IsZeroLine = false;
            myPane.YAxis.Title.Text = "Classes";
            myPane.YAxis.MinorTic.IsOpposite = false;
            myPane.YAxis.MajorTic.IsOpposite = false;
            myPane.YAxis.MinorTic.IsInside = false;
            myPane.YAxis.MajorTic.IsInside = false;
            myPane.YAxis.MinorTic.IsOutside = false;
            myPane.YAxis.MajorTic.IsOutside = false;

            myPane.XAxis.MinorTic.IsOpposite = false;
            myPane.XAxis.MajorTic.IsOpposite = false;
            myPane.XAxis.Title.IsVisible = true;
            myPane.XAxis.Title.Text = "Relative class response";
            myPane.XAxis.IsVisible = true;
            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.Max = 100;
            myPane.XAxis.IsAxisSegmentVisible = false;
            myPane.XAxis.MajorGrid.IsVisible = false;
            myPane.XAxis.MajorGrid.IsZeroLine = false;
            myPane.XAxis.MinorTic.IsOpposite = false;
            myPane.XAxis.MinorTic.IsInside = false;
            myPane.XAxis.MinorTic.IsOutside = false;
            myPane.XAxis.Scale.Format = "0'%";


            // Create data points for three BarItems using Random data
            PointPairList list = new PointPairList();

            for (int i = 0; i < discriminants.Length; i++)
                list.Add(discriminants[i] * 100, i);

            BarItem myCurve = myPane.AddBar("b", list, Color.DarkBlue);


            // Set BarBase to the YAxis for horizontal bars
            myPane.BarSettings.Base = BarBase.Y;


            zgc.AxisChange();
            zgc.Invalidate();

        }
        #endregion

        private void dgvMachines_CurrentCellChanged(object sender, EventArgs e)
        {
            DataGridViewRow row = dgvMachines.CurrentRow;
            if (row == null) return;

            KernelSupportVectorMachine m = row.Tag as KernelSupportVectorMachine;
            if (m == null) return;

            double max = m.Weights.Max();
            double min = m.Weights.Min();

            dgvVectors.Rows.Clear();
            for (int i = 0; i < m.SupportVectors.Length; i++)
            {
                var vector = m.SupportVectors[i];
                var weight = m.Weights[i];
                double[] f = vector.Apply(x => x *
                       Accord.Math.Tools.Scale(min, max, -1, 1, weight));
                dgvVectors.Rows.Add(Export(f), m.Weights[i]);
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (cbContinuous.Checked && e.Button == MouseButtons.Left)
                btnCanvasClassify_Click(this, EventArgs.Empty);
        }

        private void btnEstimate_Click(object sender, EventArgs e)
        {
            // Extract inputs
            int rows = dgvTrainingSource.Rows.Count;
            double[][] input = new double[rows][];
            for (int i = 0; i < rows; i++)
                input[i] = (double[])dgvTrainingSource.Rows[i].Cells["colTrainingFeatures"].Value;

            Gaussian g = Gaussian.Estimate(input, input.Length / 4);

            numSigma.Value = (decimal)g.Sigma;
        }

        private void btnEstimateC_Click(object sender, EventArgs e)
        {
            // Extract inputs
            int rows = dgvTrainingSource.Rows.Count;
            double[][] input = new double[rows][];
            for (int i = 0; i < rows; i++)
                input[i] = (double[])dgvTrainingSource.Rows[i].Cells["colTrainingFeatures"].Value;

            IKernel kernel;
            if (rbGaussian.Checked)
                kernel = new Gaussian((double)numSigma.Value);
            else
                kernel = new Polynomial((int)numDegree.Value, (double)numConstant.Value);

            numComplexity.Value = (decimal)SequentialMinimalOptimization.EstimateComplexity(kernel, input);
        }




    }
}
