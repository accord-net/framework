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

using Accord;
using Accord.Controls;
using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Analysis;
using Accord.Statistics.Formats;
using Accord.Statistics.Kernels;
using AForge;
using Components;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ZedGraph;

namespace SVMs
{
    public partial class MainForm : Form
    {

        private KernelSupportVectorMachine svm;

        string[] sourceColumns;



        public MainForm()
        {
            InitializeComponent();

            dgvLearningSource.AutoGenerateColumns = true;
            dgvPerformance.AutoGenerateColumns = false;

            openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Resources");
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
                        }
                    }
                }
            }
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

            // Create the specified Kernel
            IKernel kernel = getKernel();

            double[,] sourceMatrix;
            double[,] inputs;
            int[] labels;
            getData(out sourceMatrix, out inputs, out labels);

            // Perform classification
            SequentialMinimalOptimization smo;

            // Creates the Support Vector Machine using the selected kernel
            svm = new KernelSupportVectorMachine(kernel, 2);

            // Creates a new instance of the SMO Learning Algorithm
            smo = new SequentialMinimalOptimization(svm, inputs.ToArray(), labels);

            // Set learning parameters
            smo.Complexity = (double)numC.Value;
            smo.Tolerance = (double)numT.Value;
            smo.PositiveWeight = (double)numPositiveWeight.Value;
            smo.NegativeWeight = (double)numNegativeWeight.Value;

            bool converged = true;

            try
            {
                // Run
                double error = smo.Run();
            }
            catch (ConvergenceException)
            {
                converged = false;
            }

            if (converged)
                toolStripStatusLabel1.Text = "Convergence reached.";
            else
                toolStripStatusLabel1.Text = "Convergence could not be attained.";


            // Show support vectors
            double[,] supportVectors = svm.SupportVectors.ToMatrix();
            double[,] supportVectorsWeights = supportVectors.InsertColumn(
                svm.Weights, supportVectors.GetLength(1));

            if (supportVectors.GetLength(0) == 0)
            {
                dgvSupportVectors.DataSource = null;
                graphSupportVectors.GraphPane.CurveList.Clear();
                return;
            }

            dgvSupportVectors.DataSource = new ArrayDataView(supportVectorsWeights,
                sourceColumns.Submatrix(0, supportVectors.GetLength(1) - 1).Concatenate("Weight"));

            double[,] graph = supportVectors;

            int[] idx = new int[svm.SupportVectors.Length];
            double[] a = sourceMatrix.GetColumn(0);
            double[] o = sourceMatrix.GetColumn(2);
            for (int i = 0; i < idx.Length; i++)
                idx[i] = Matrix.Find(a, x => x == svm.SupportVectors[i][0], true)[0];
            graph = graph.InsertColumn(o.Submatrix(idx), 2);

            // Plot support vectors
            CreateScatterplot(graphSupportVectors, graph);

            var ranges = Matrix.Range(sourceMatrix, 0);
            double[][] map = Matrix.CartesianProduct(
                Matrix.Interval(ranges[0], 0.05),
                Matrix.Interval(ranges[1], 0.05));

            var result = map.Apply(svm.Compute).Apply(Math.Sign);

            var graph2 = map.ToMatrix().InsertColumn(result.ToDouble());

            CreateScatterplot(zedGraphControl2, graph2);
        }

        private void getData(out double[,] sourceMatrix, out double[,] inputs, out int[] labels)
        {
            // Creates a matrix from the source data table
            sourceMatrix = (dgvLearningSource.DataSource as DataTable).ToMatrix(out sourceColumns);

            // Get only the input vector values
            inputs = sourceMatrix.Submatrix(0, sourceMatrix.GetLength(0) - 1, 0, 1);

            // Get only the label outputs
            labels = new int[sourceMatrix.GetLength(0)];
            for (int i = 0; i < labels.Length; i++)
                labels[i] = (int)sourceMatrix[i, 2];
        }


        private void btnTestingRun_Click(object sender, EventArgs e)
        {
            if (svm == null || dgvTestingSource.DataSource == null)
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
                output[i] = System.Math.Sign(svm.Compute(inputs[i]));

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
            //myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45.0f);
            //myPane.Fill = new Fill(Color.White, Color.SlateGray, 45.0f);
            myPane.Fill = new Fill(Color.WhiteSmoke);

            zgc.AxisChange();
            zgc.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Creates a matrix from the source data table
            double[,] sourceMatrix = (dgvLearningSource.DataSource as DataTable).ToMatrix(out sourceColumns);

            // Get only the input vector values
            var inputs = sourceMatrix.Submatrix(0, sourceMatrix.GetLength(0) - 1, 0, 1).ToArray();

            // Get only the label outputs
            var outputs = new int[sourceMatrix.GetLength(0)];
            for (int i = 0; i < outputs.Length; i++)
                outputs[i] = (int)sourceMatrix[i, 2];

            var cv = new CrossValidation<KernelSupportVectorMachine>(inputs.Length, 10);
            cv.Fitting = (int k, int[] training, int[] testing) =>
            {
                var trainingInputs = inputs.Submatrix(training);
                var trainingOutputs = outputs.Submatrix(training);
                var testingInputs = inputs.Submatrix(testing);
                var testingOutputs = outputs.Submatrix(testing);

                // Create the specified Kernel
                IKernel kernel = getKernel();


                // Creates the Support Vector Machine using the selected kernel
                var svm = new KernelSupportVectorMachine(kernel, 2);

                // Creates a new instance of the SMO Learning Algortihm
                var smo = new SequentialMinimalOptimization(svm, trainingInputs, trainingOutputs);

                // Set learning parameters
                smo.Complexity = (double)numC.Value;
                smo.Tolerance = (double)numT.Value;

                // Run
                double trainingError = smo.Run();
                double validationError = smo.ComputeError(testingInputs, testingOutputs);

                return new CrossValidationValues<KernelSupportVectorMachine>(svm, trainingError, validationError);

            };

            var result = cv.Compute();

        }


        private IKernel getKernel()
        {
            if (rbGaussian.Checked)
            {
                return new Gaussian((double)numSigma.Value);
            }
            else if (rbPolynomial.Checked)
            {
                if (numDegree.Value == 1)
                    return new Linear((double)numPolyConstant.Value);
                return new Polynomial((int)numDegree.Value, (double)numPolyConstant.Value);
            }
            else if (rbLaplacian.Checked)
            {
                return new Laplacian((double)numLaplacianSigma.Value);
            }
            else if (rbSigmoid.Checked)
            {
                return new Sigmoid((double)numSigAlpha.Value, (double)numSigB.Value);
            }
            else throw new Exception();
        }

        private void btnEstimateGaussian_Click(object sender, EventArgs e)
        {
            double[,] sourceMatrix;
            double[,] inputs;
            int[] labels;
            getData(out sourceMatrix, out inputs, out labels);
            DoubleRange range;

            var g = Gaussian.Estimate(inputs.ToArray(), labels.Length, out range);

            numSigma.Value = (decimal)g.Sigma;
        }

        private void btnEstimateLaplacian_Click(object sender, EventArgs e)
        {
            double[,] sourceMatrix;
            double[,] inputs;
            int[] labels;
            getData(out sourceMatrix, out inputs, out labels);
            DoubleRange range;

            var g = Laplacian.Estimate(inputs.ToArray(), labels.Length, out range);

            numLaplacianSigma.Value = (decimal)g.Sigma;
        }

        private void btnEstimateSig_Click(object sender, EventArgs e)
        {
            double[,] sourceMatrix;
            double[,] inputs;
            int[] labels;
            getData(out sourceMatrix, out inputs, out labels);
            DoubleRange range;

            var g = Sigmoid.Estimate(inputs.ToArray(), labels.Length, out range);

            if (g.Alpha < (double)Decimal.MaxValue && g.Alpha > (double)Decimal.MinValue)
                numSigAlpha.Value = (decimal)g.Alpha;

            if (g.Constant < (double)Decimal.MaxValue && g.Constant > (double)Decimal.MinValue)
            numSigB.Value = (decimal)g.Constant;
        }

        private void btnEstimateC_Click(object sender, EventArgs e)
        {
            double[,] sourceMatrix;
            double[,] inputs;
            int[] labels;
            getData(out sourceMatrix, out inputs, out labels);

            IKernel kernel = getKernel();
            numC.Value = (decimal)SequentialMinimalOptimization.EstimateComplexity(kernel, inputs.ToArray());
        }
    }
}
