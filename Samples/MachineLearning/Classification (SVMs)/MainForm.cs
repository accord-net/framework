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
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Accord;
using Accord.Controls;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Analysis;
using Accord.Statistics.Formats;
using Accord.Statistics.Kernels;
using AForge;
using Components;
using ZedGraph;

namespace Classification.SVMs
{
    /// <summary>
    ///   Classification sample application using Kernel Support Vector Machines.
    /// </summary>
    /// 
    public partial class MainForm : Form
    {

        KernelSupportVectorMachine svm;

        string[] columnNames; // stores the column names for the loaded data



        public MainForm()
        {
            InitializeComponent();

            dgvLearningSource.AutoGenerateColumns = true;
            dgvPerformance.AutoGenerateColumns = false;

            openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Resources");
        }



        /// <summary>
        ///   Creates a Support Vector Machine and teaches it to recognize
        ///   the previously loaded dataset using the current UI settings.
        /// </summary>
        /// 
        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (dgvLearningSource.DataSource == null)
            {
                MessageBox.Show("Please load some data first.");
                return;
            }

            // Finishes and save any pending changes to the given data
            dgvLearningSource.EndEdit();



            // Creates a matrix from the entire source data table
            double[,] table = (dgvLearningSource.DataSource as DataTable).ToMatrix(out columnNames);

            // Get only the input vector values (first two columns)
            double[][] inputs = table.GetColumns(0, 1).ToArray();

            // Get only the output labels (last column)
            int[] outputs = table.GetColumn(2).ToInt32();


            // Create the specified Kernel
            IKernel kernel = createKernel();


            // Creates the Support Vector Machine for 2 input variables
            svm = new KernelSupportVectorMachine(kernel, inputs: 2);

            // Creates a new instance of the SMO learning algorithm
            var smo = new SequentialMinimalOptimization(svm, inputs, outputs)
            {
                // Set learning parameters
                Complexity = (double)numC.Value,
                Tolerance = (double)numT.Value,
                PositiveWeight = (double)numPositiveWeight.Value,
                NegativeWeight = (double)numNegativeWeight.Value
            };


            try
            {
                // Run
                double error = smo.Run();

                lbStatus.Text = "Training complete!";
            }
            catch (ConvergenceException)
            {
                lbStatus.Text = "Convergence could not be attained. "+
                    "The learned machine might still be usable.";
            }


            // Check if we got support vectors
            if (svm.SupportVectors.Length == 0)
            {
                dgvSupportVectors.DataSource = null;
                graphSupportVectors.GraphPane.CurveList.Clear();
                return;
            }



            // Show support vectors on the Support Vectors tab page
            double[][] supportVectorsWeights = svm.SupportVectors.InsertColumn(svm.Weights);

            string[] supportVectorNames = columnNames.RemoveAt(columnNames.Length - 1).Concatenate("Weight");
            dgvSupportVectors.DataSource = new ArrayDataView(supportVectorsWeights, supportVectorNames);



            // Show the support vector labels on the scatter plot
            double[] supportVectorLabels = new double[svm.SupportVectors.Length];
            for (int i = 0; i < supportVectorLabels.Length; i++)
            {
                int j = inputs.Find(sv => sv == svm.SupportVectors[i])[0];
                supportVectorLabels[i] = outputs[j];
            }

            double[][] graph = svm.SupportVectors.InsertColumn(supportVectorLabels);

            CreateScatterplot(graphSupportVectors, graph.ToMatrix());



            // Get the ranges for each variable (X and Y)
            DoubleRange[] ranges = Matrix.Range(table, 0);

            // Generate a Cartesian coordinate system
            double[][] map = Matrix.CartesianProduct(
                Matrix.Interval(ranges[0], 0.05),
                Matrix.Interval(ranges[1], 0.05));

            // Classify each point in the Cartesian coordinate system
            double[] result = map.Apply(svm.Compute).Apply(Math.Sign).ToDouble();
            double[,] surface = map.ToMatrix().InsertColumn(result);

            CreateScatterplot(zedGraphControl2, surface);
        }


        /// <summary>
        ///   Tests the previously created machine into a new set of data.
        /// </summary>
        /// 
        private void btnTestingRun_Click(object sender, EventArgs e)
        {
            if (svm == null || dgvTestingSource.DataSource == null)
            {
                MessageBox.Show("Please create a machine first.");
                return;
            }


            // Creates a matrix from the source data table
            double[,] table = (dgvTestingSource.DataSource as DataTable).ToMatrix();


            // Extract the first and second columns (X and Y)
            double[][] inputs = table.GetColumns(0, 1).ToArray();

            // Extract the expected output labels
            int[] expected = table.GetColumn(2).ToInt32();


            int[] output = new int[expected.Length];

            // Compute the actual machine outputs
            for (int i = 0; i < expected.Length; i++)
                output[i] = System.Math.Sign(svm.Compute(inputs[i]));



            // Use confusion matrix to compute some performance metrics
            ConfusionMatrix confusionMatrix = new ConfusionMatrix(output, expected, 1, -1);
            dgvPerformance.DataSource = new [] { confusionMatrix };


            // Create performance scatter plot
            CreateResultScatterplot(zedGraphControl1, inputs, expected.ToDouble(), output.ToDouble());
        }



        private void btnEstimateGaussian_Click(object sender, EventArgs e)
        {
            DataTable source = dgvLearningSource.DataSource as DataTable;

            // Creates a matrix from the source data table
            double[,] sourceMatrix = source.ToMatrix(out columnNames);

            // Get only the input vector values (in the first two columns)
            double[][] inputs = sourceMatrix.GetColumns(0, 1).ToArray();

            DoubleRange range; // valid range will be returned as an out parameter
            Gaussian gaussian = Gaussian.Estimate(inputs, inputs.Length, out range);

            numSigma.Value = (decimal)gaussian.Sigma;
        }

        private void btnEstimateLaplacian_Click(object sender, EventArgs e)
        {
            DataTable source = dgvLearningSource.DataSource as DataTable;

            // Creates a matrix from the source data table
            double[,] sourceMatrix = source.ToMatrix(out columnNames);

            // Get only the input vector values (in the first two columns)
            double[][] inputs = sourceMatrix.GetColumns(0, 1).ToArray();

            DoubleRange range; // valid range will be returned as an out parameter
            var laplacian = Laplacian.Estimate(inputs, inputs.Length, out range);

            numLaplacianSigma.Value = (decimal)laplacian.Sigma;
        }

        private void btnEstimateSigmoid_Click(object sender, EventArgs e)
        {
            DataTable source = dgvLearningSource.DataSource as DataTable;

            // Creates a matrix from the source data table
            double[,] sourceMatrix = source.ToMatrix(out columnNames);

            // Get only the input vector values (in the first two columns)
            double[][] inputs = sourceMatrix.GetColumns(0, 1).ToArray();

            DoubleRange range; // valid range will be returned as an out parameter
            var sigmoid = Sigmoid.Estimate(inputs, inputs.Length, out range);

            if (sigmoid.Alpha < (double)Decimal.MaxValue && sigmoid.Alpha > (double)Decimal.MinValue)
                numSigAlpha.Value = (decimal)sigmoid.Alpha;

            if (sigmoid.Constant < (double)Decimal.MaxValue && sigmoid.Constant > (double)Decimal.MinValue)
                numSigB.Value = (decimal)sigmoid.Constant;
        }

        private void btnEstimateC_Click(object sender, EventArgs e)
        {
            DataTable source = dgvLearningSource.DataSource as DataTable;

            // Creates a matrix from the source data table
            double[,] sourceMatrix = source.ToMatrix(out columnNames);

            // Get only the input vector values (in the first two columns)
            double[][] inputs = sourceMatrix.GetColumns(0, 1).ToArray();

            IKernel kernel = createKernel();

            // Estimate a suitable value for SVM's complexity parameter C
            double c = SequentialMinimalOptimization.EstimateComplexity(kernel, inputs);

            numC.Value = (decimal)c;
        }


        /// <summary>
        ///   Creates the kernel function specified in the user interface.
        /// </summary>
        /// 
        private IKernel createKernel()
        {
            if (rbGaussian.Checked)
                return new Gaussian((double)numSigma.Value);

            if (rbPolynomial.Checked)
            {
                if (numDegree.Value == 1)
                    return new Linear((double)numPolyConstant.Value);
                return new Polynomial((int)numDegree.Value, (double)numPolyConstant.Value);
            }

            if (rbLaplacian.Checked)
                return new Laplacian((double)numLaplacianSigma.Value);

            if (rbSigmoid.Checked)
                return new Sigmoid((double)numSigAlpha.Value, (double)numSigB.Value);

            else throw new Exception();
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

                        double[,] sourceMatrix = tableSource.ToMatrix(out columnNames);

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
                        }
                    }
                }
            }

            lbStatus.Text = "Switch to the Machine Creation tab to create a learning machine!";
        }





        #region Scatter plot and Graph creation
        public void CreateScatterplot(ZedGraphControl zgc, double[,] graph)
        {
            GraphPane myPane = zgc.GraphPane;
            myPane.CurveList.Clear();

            // Set the titles
            myPane.Title.IsVisible = false;
            myPane.XAxis.Title.Text = columnNames[0];
            myPane.YAxis.Title.Text = columnNames[1];


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
            LineItem myCurve = myPane.AddCurve("G1", list1, Color.Blue, SymbolType.Diamond);
            myCurve.Line.IsVisible = false;
            myCurve.Symbol.Border.IsVisible = false;
            myCurve.Symbol.Fill = new Fill(Color.Blue);

            myCurve = myPane.AddCurve("G2", list2, Color.Green, SymbolType.Diamond);
            myCurve.Line.IsVisible = false;
            myCurve.Symbol.Border.IsVisible = false;
            myCurve.Symbol.Fill = new Fill(Color.Green);


            // Fill the background of the chart rect and pane
            //myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45.0f);
            //myPane.Fill = new Fill(Color.White, Color.SlateGray, 45.0f);
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
            myPane.XAxis.Title.Text = columnNames[0];
            myPane.YAxis.Title.Text = columnNames[1];



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
            //myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45.0f);
            //myPane.Fill = new Fill(Color.White, Color.SlateGray, 45.0f);
            myPane.Fill = new Fill(Color.WhiteSmoke);

            zgc.AxisChange();
            zgc.Invalidate();
        }
        #endregion


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
