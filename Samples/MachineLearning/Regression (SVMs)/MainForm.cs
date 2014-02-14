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
using System.Linq;
using System.Windows.Forms;
using Accord;
using Accord.Controls;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Formats;
using Accord.Statistics.Kernels;
using AForge;
using Components;
using ZedGraph;

namespace Regression.SVMs
{
    public partial class MainForm : Form
    {

        KernelSupportVectorMachine svm;

        string[] columnNames;



        public MainForm()
        {
            InitializeComponent();

            dgvLearningSource.AutoGenerateColumns = true;
            dgvPerformance.AutoGenerateColumns = false;

            openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Resources");
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



            // Creates a matrix from the entire source data table
            double[,] table = (dgvLearningSource.DataSource as DataTable).ToMatrix(out columnNames);

            // Get only the input vector values (first two columns)
            double[][] inputs = table.GetColumns(0).ToArray();

            // Get only the outputs (last column)
            double[] outputs = table.GetColumn(1);


            // Create the specified Kernel
            IKernel kernel = createKernel();


            // Create the Support Vector Machine for 1 input variable
            svm = new KernelSupportVectorMachine(kernel, inputs: 1);

            // Creates a new instance of the SMO for regression learning algorithm
            var smo = new SequentialMinimalOptimizationRegression(svm, inputs, outputs)
            {
                // Set learning parameters
                Complexity = (double)numC.Value,
                Tolerance = (double)numT.Value,
                Epsilon = (double)numEpsilon.Value
            };



            try
            {
                // Run
                double error = smo.Run();

                lbStatus.Text = "Training complete!";
            }
            catch (ConvergenceException)
            {
                lbStatus.Text = "Convergence could not be attained. " +
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
            DoubleRange range = Matrix.Range(table.GetColumn(0));

            double[][] map = Matrix.Interval(range, 0.05).ToArray();

            // Classify each point in the Cartesian coordinate system
            double[] result = map.Apply(svm.Compute);
            double[,] surface = map.ToMatrix().InsertColumn(result);

            CreateScatterplot(zedGraphControl2, surface);
        }


        private void btnTestingRun_Click(object sender, EventArgs e)
        {
            if (svm == null || dgvTestingSource.DataSource == null)
            {
                MessageBox.Show("Please create a machine first.");
                return;
            }


            // Creates a matrix from the source data table
            double[,] table = (dgvTestingSource.DataSource as DataTable).ToMatrix();


            // Extract the first columns (X)
            double[][] inputs = table.GetColumns(0).ToArray();

            // Extract the expected output values
            double[] expected = table.GetColumn(1);

            // Compute the actual machine outputs
            var output = new double[expected.Length];
            for (int i = 0; i < expected.Length; i++)
                output[i] = svm.Compute(inputs[i]);


            // Compute R² and Sum-of-squares error
            double rSquared = Accord.Statistics.Tools.Determination(output, expected);
            double error = expected.Subtract(output).ElementwisePower(2).Sum() / output.Length;


            // Anonymous magic! :D
            var r = new { RSquared = rSquared, Error = error };
            dgvPerformance.DataSource = (new[] { r }).ToList();


            // Create performance scatter plot
            CreateResultScatterplot(zedGraphControl1, inputs, expected, output);
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
                        this.dgvLearningSource.DataSource = tableSource;
                        this.dgvTestingSource.DataSource = tableSource.Copy();

                        double[,] sourceMatrix = tableSource.ToMatrix(out columnNames);

                        CreateScatterplot(graphInput, sourceMatrix);
                    }
                }
            }
        }



        public void CreateScatterplot(ZedGraphControl zgc, double[,] graph)
        {
            GraphPane myPane = zgc.GraphPane;
            myPane.CurveList.Clear();

            // Set the titles
            myPane.Title.IsVisible = false;
            myPane.XAxis.Title.Text = columnNames[0];
            myPane.YAxis.Title.Text = columnNames[1];


            // Regression problem
            PointPairList list1 = new PointPairList();
            for (int i = 0; i < graph.GetLength(0); i++)
                list1.Add(graph[i, 0], graph[i, 1]);

            // Add the curve
            LineItem myCurve = myPane.AddCurve("Y", list1, Color.Blue, SymbolType.Diamond);
            myCurve.Line.IsVisible = false;
            myCurve.Symbol.Border.IsVisible = false;
            myCurve.Symbol.Fill = new Fill(Color.Blue);

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
            myPane.XAxis.Title.Text = columnNames[0];
            myPane.YAxis.Title.Text = columnNames[1];


            // Regression problem
            PointPairList list1 = new PointPairList(); // svm output
            PointPairList list2 = new PointPairList(); // expected output
            for (int i = 0; i < inputs.Length; i++)
            {
                list1.Add(inputs[i][0], output[i]);
                list2.Add(inputs[i][0], expected[i]);
            }

            // Add the curve
            LineItem myCurve = myPane.AddCurve("Model output", list1, Color.Blue, SymbolType.Diamond);
            myCurve.Line.IsVisible = true;
            myCurve.Symbol.Border.IsVisible = false;
            myCurve.Symbol.Fill = new Fill(Color.Blue);

            myCurve = myPane.AddCurve("Data output", list2, Color.Red, SymbolType.Diamond);
            myCurve.Line.IsVisible = false;
            myCurve.Symbol.Border.IsVisible = false;
            myCurve.Symbol.Fill = new Fill(Color.Red);

            // Fill the background of the chart rect and pane
            myPane.Fill = new Fill(Color.WhiteSmoke);

            zgc.AxisChange();
            zgc.Invalidate();
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
