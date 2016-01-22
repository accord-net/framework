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

using Accord;
using Accord.IO;
using Accord.Math;
using Accord.Statistics.Analysis;
using AForge;
using Components;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ZedGraph;

namespace Classification.MLR
{
    /// <summary>
    ///   Classification sample application using Multinomial Logistic Regression Analysis.
    /// </summary>
    /// 
    public partial class MainForm : Form
    {

        MultinomialLogisticRegressionAnalysis mlr;

        string[] columnNames; // stores the column names for the loaded data
        string[] inputNames;


        public MainForm()
        {
            InitializeComponent();

            dgvLearningSource.AutoGenerateColumns = true;

            openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Resources");
        }



        /// <summary>
        ///   Creates a Multinomial Logistic Regression Analysis.
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
            int[] outputs = table.GetColumn(2).Subtract(1).ToInt32();




            // Create and compute a new Simple Descriptive Analysis
            var sda = new DescriptiveAnalysis(table, columnNames);

            sda.Compute();

            // Show the descriptive analysis on the screen
            dgvDistributionMeasures.DataSource = sda.Measures;




            // Creates the Support Vector Machine for 2 input variables
            mlr = new MultinomialLogisticRegressionAnalysis(inputs, outputs);

            try
            {
                // Run
                mlr.Compute();

                lbStatus.Text = "Analysis complete!";
            }
            catch (ConvergenceException)
            {
                lbStatus.Text = "Convergence could not be attained. " +
                    "The learned machine might still be usable.";
            }


            createSurface(table);

            // Populate details about the fitted model
            tbChiSquare.Text = mlr.ChiSquare.Statistic.ToString("N5");
            tbPValue.Text = mlr.ChiSquare.PValue.ToString("N5");
            checkBox1.Checked = mlr.ChiSquare.Significant;
            tbDeviance.Text = mlr.Deviance.ToString("N5");
            tbLogLikelihood.Text = mlr.LogLikelihood.ToString("N5");

            dgvCoefficients.DataSource = mlr.Coefficients;
        }

        private void createSurface(double[,] table)
        {
            // Get the ranges for each variable (X and Y)
            DoubleRange[] ranges = table.GetRange(0);

            // Generate a Cartesian coordinate system
            double[][] map = Matrix.Cartesian(
                Vector.Interval(ranges[0], 0.05),
                Vector.Interval(ranges[1], 0.05));

            var lr = mlr.Regression;

            // Classify each point in the Cartesian coordinate system
            double[] result = map.Apply(x =>
            {
                int imax; lr.Compute(x).Max(out imax); 
                return imax; 
            }).ToDouble();

            double[,] surface = map.ToMatrix().InsertColumn(result);

            decisionMap.DataSource = surface;
        }


        /// <summary>
        ///   Tests the previously created machine into a new set of data.
        /// </summary>
        /// 
        private void btnTestingRun_Click(object sender, EventArgs e)
        {
            if (mlr == null || dgvTestingSource.DataSource == null)
            {
                MessageBox.Show("Please create a machine first.");
                return;
            }


            // Creates a matrix from the source data table
            double[,] table = (dgvTestingSource.DataSource as DataTable).ToMatrix();


            // Extract the first and second columns (X and Y)
            double[][] inputs = table.GetColumns(0, 1).ToArray();

            // Extract the expected output labels
            int[] expected = table.GetColumn(2).Subtract(1).ToInt32();


            int[] output = new int[expected.Length];

            // Compute the actual machine outputs
            for (int i = 0; i < expected.Length; i++)
                mlr.Regression.Compute(inputs[i]).Max(out output[i]);



            // Use confusion matrix to compute some performance metrics
            var confusionMatrix = new GeneralConfusionMatrix(mlr.OutputCount, output, expected);
            dgvPerformance.DataSource = new[] { confusionMatrix };


            resultsView.DataSource = inputs.InsertColumn(output);
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

                            double[,] graph = tableSource.ToMatrix(out columnNames);
                            graphInput.DataSource = graph;

                            inputNames = columnNames.Submatrix(2);

                            lbStatus.Text = "Now, click 'Run analysis' to start processing the data!";
                        }
                    }
                }
            }
        }


        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {
            if (dgvDistributionMeasures.CurrentRow != null)
            {
                DataGridViewRow row = (DataGridViewRow)dgvDistributionMeasures.CurrentRow;
                dataHistogramView1.DataSource =
                    ((DescriptiveMeasures)row.DataBoundItem).Samples;
            }
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
