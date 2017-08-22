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

using Accord;
using Accord.Controls;
using Accord.IO;
using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Math.Distances;
using Accord.Statistics;
using Accord.Statistics.Analysis;
using Accord.Statistics.Distributions.Fitting;
using Accord.Statistics.Kernels;
using AForge;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ZedGraph;

using TClustering = Accord.MachineLearning.IMulticlassClassifier<double[], int>;
using TLearning = Accord.MachineLearning.IUnsupervisedLearning<
    Accord.MachineLearning.IMulticlassClassifier<double[], int>, double[], int>;

namespace SampleApp
{
    /// <summary>
    ///   Classification sample application using Kernel Support Vector Machines.
    /// </summary>
    /// 
    public partial class MainForm : Form
    {

        TLearning learning;
        TClustering clustering;

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
            double[][] inputs = table.GetColumns(0, 1).ToJagged();


            try
            {
                // Create and run the specified algorithm
                this.learning = createClustering(inputs);

                this.clustering = this.learning.Learn(inputs);

                lbStatus.Text = "Training complete!";
            }
            catch (ConvergenceException)
            {
                lbStatus.Text = "Convergence could not be attained. " +
                    "The learned clustering might still be usable.";
            }

            createSurface(table);
        }

        private void createSurface(double[,] table)
        {
            // Get the ranges for each variable (X and Y)
            DoubleRange[] ranges = table.GetRange(0);

            // Generate a Cartesian coordinate system
            double[][] map = Matrix.Cartesian(
                Vector.Interval(ranges[0], 0.05),
                Vector.Interval(ranges[1], 0.05));

            // Classify each point in the Cartesian coordinate system
            double[] result = clustering.Decide(map).ToDouble();
            double[,] surface = map.ToMatrix().InsertColumn(result);

            scatterplotView3.DataSource = surface;
        }


        /// <summary>
        ///   Tests the previously created machine into a new set of data.
        /// </summary>
        /// 
        private void btnTestingRun_Click(object sender, EventArgs e)
        {
            if (clustering == null || dgvTestingSource.DataSource == null)
            {
                MessageBox.Show("Please create a machine first.");
                return;
            }

            // Creates a matrix from the source data table
            double[,] table = (dgvTestingSource.DataSource as DataTable).ToMatrix();

            // Extract the first and second columns (X and Y)
            double[][] inputs = table.GetColumns(0, 1).ToJagged();

            // Extract the expected output labels
            int[] expected = table.GetColumn(2).ToZeroOne();
            int[] output;

            // Compute cluster decisions for each point
            if (this.learning is BalancedKMeans)
            {
                output = (learning as BalancedKMeans).Labels;
            }
            else
            {
                output = clustering.Decide(inputs);
            }

            // Use confusion matrix to compute some performance metrics
            var confusionMatrix = new ConfusionMatrix(output, expected, 1, 0);
            dgvPerformance.DataSource = new[] { confusionMatrix };

            // Create performance scatter plot
            scatterplotView2.DataSource = inputs.InsertColumn(output);
        }




        /// <summary>
        ///   Creates the clustering algorithm.
        /// </summary>
        /// 
        private TLearning createClustering(double[][] data)
        {
            if (rbKMeans.Checked)
            {
                if (cbBalanced.Checked)
                {
                    return new BalancedKMeans((int)numKMeans.Value)
                    {
                        MaxIterations = 1000,
                    };
                }

                return new KMeans((int)numKMeans.Value);
            }


            if (rbMeanShift.Checked)
            {
                var kernel = new Accord.Statistics.Distributions.DensityKernels.GaussianKernel(2);
                return new MeanShift()
                {
                    Kernel = kernel,
                    Bandwidth = (double)numRadius.Value
                };
            }

            if (rbGMM.Checked)
            {
                return new GaussianMixtureModel((int)numGaussians.Value)
                {
                    Options = new NormalOptions()
                    {
                        Regularization = 1e-10
                    }
                };
            }

            throw new InvalidOperationException("Invalid options");
        }



        private void MenuFileOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string filename = openFileDialog.FileName;
                string extension = Path.GetExtension(filename);
                if (extension == ".xls" || extension == ".xlsx")
                {
                    var db = new ExcelReader(filename, true, false);
                    var t = new TableSelectDialog(db.GetWorksheetList());

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

                            scatterplotView1.DataSource = sourceMatrix;
                        }
                    }
                }
            }

            lbStatus.Text = "Switch to the Machine Creation tab to create a learning machine!";
        }



        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }

        private void numPolyConstant_ValueChanged(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }
    }
}
