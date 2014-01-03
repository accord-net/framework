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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Accord.MachineLearning;
using Accord.Math;
using Accord.Statistics.Formats;
using Accord.Statistics.Models.Regression.Linear;
using Components;
using ZedGraph;

namespace Fitting.RANSAC
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();

            openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Resources");
        }



        private void btnCompute_Click(object sender, EventArgs e)
        {
            DataTable dataTable = dgvAnalysisSource.DataSource as DataTable;

            if (dataTable == null)
                return;

            // Gather the available data
            double[][] data = dataTable.ToArray();


            // First, fit simple linear regression directly for comparison reasons.
            double[] x = data.GetColumn(0); // Extract the independent variable
            double[] y = data.GetColumn(1); // Extract the dependent variable

            // Create a simple linear regression
            var regression = new SimpleLinearRegression();

            // Estimate a line passing through the (x, y) points
            double sumOfSquaredErrors = regression.Regress(x, y);

            // Now, compute the values predicted by the 
            // regression for the original input points
            double[] commonOutput = regression.Compute(x);


            // Now, fit simple linear regression using RANSAC
            int maxTrials = (int)numMaxTrials.Value;
            int minSamples = (int)numSamples.Value;
            double probability = (double)numProbability.Value;
            double errorThreshold = (double)numThreshold.Value;

            // Create a RANSAC algorithm to fit a simple linear regression
            var ransac = new RANSAC<SimpleLinearRegression>(minSamples)
            {
                Probability = probability,
                Threshold = errorThreshold,
                MaxEvaluations = maxTrials,

                // Define a fitting function
                Fitting = delegate(int[] sample)
                {
                    // Retrieve the training data
                    double[] inputs = x.Submatrix(sample);
                    double[] outputs = y.Submatrix(sample);

                    // Build a Simple Linear Regression model
                    var r = new SimpleLinearRegression();
                    r.Regress(inputs, outputs);
                    return r;
                },

                // Define a check for degenerate samples
                Degenerate = delegate(int[] sample)
                {
                    // In this case, we will not be performing such checks.
                    return false;
                },

                // Define a inlier detector function
                Distances = delegate(SimpleLinearRegression r, double threshold)
                {
                    List<int> inliers = new List<int>();
                    for (int i = 0; i < x.Length; i++)
                    {
                        // Compute error for each point
                        double error = r.Compute(x[i]) - y[i];

                        // If the squared error is below the given threshold,
                        //  the point is considered to be an inlier.
                        if (error * error < threshold)
                            inliers.Add(i);
                    }

                    return inliers.ToArray();
                }
            };


            // Now that the RANSAC hyperparameters have been specified, we can 
            // compute another regression model using the RANSAC algorithm:

            int[] inlierIndices;
            SimpleLinearRegression robustRegression = ransac.Compute(data.Length, out inlierIndices);


            if (robustRegression == null)
            {
                lbStatus.Text = "RANSAC failed. Please try again after adjusting its parameters.";
                return; // the RANSAC algorithm did not find any inliers and no model was created
            }



            // Compute the output of the model fitted by RANSAC
            double[] ransacOutput = robustRegression.Compute(x);

            // Create scatter plot comparing the outputs from the standard
            //  linear regression and the RANSAC-fitted linear regression.
            CreateScatterplot(graphInput, x, y, commonOutput, ransacOutput,
                x.Submatrix(inlierIndices), y.Submatrix(inlierIndices));

            lbStatus.Text = "Regression created! Please compare the RANSAC "
                + "regression (blue) with the simple regression (in red).";
        }







        public void CreateScatterplot(ZedGraphControl zgc, double[] x, double[] y, double[] slr, double[] rlr,
            double[] inliersX, double[] inliersY)
        {
            GraphPane myPane = zgc.GraphPane;
            myPane.CurveList.Clear();

            // Set the titles
            myPane.Title.IsVisible = false;
            myPane.Chart.Border.IsVisible = false;
            myPane.XAxis.Title.Text = "X";
            myPane.YAxis.Title.Text = "Y";
            myPane.XAxis.IsAxisSegmentVisible = true;
            myPane.YAxis.IsAxisSegmentVisible = true;
            myPane.XAxis.MinorGrid.IsVisible = false;
            myPane.YAxis.MinorGrid.IsVisible = false;
            myPane.XAxis.MinorTic.IsOpposite = false;
            myPane.XAxis.MajorTic.IsOpposite = false;
            myPane.YAxis.MinorTic.IsOpposite = false;
            myPane.YAxis.MajorTic.IsOpposite = false;
            myPane.XAxis.Scale.MinGrace = 0;
            myPane.XAxis.Scale.MaxGrace = 0;
            myPane.YAxis.Scale.MinGrace = 0;
            myPane.YAxis.Scale.MaxGrace = 0;


            PointPairList list1 = new PointPairList(x, y);
            PointPairList list2 = null;
            PointPairList list3 = null;
            PointPairList list4 = new PointPairList(inliersX, inliersY);

            if (slr != null)
                list2 = new PointPairList(x, slr);
            if (rlr != null)
                list3 = new PointPairList(x, rlr);
            if (inliersX != null)
                list4 = new PointPairList(inliersX, inliersY);

            LineItem myCurve;

            // Add the curves
            myCurve = myPane.AddCurve("Inliers", list4, Color.Blue, SymbolType.Circle);
            myCurve.Line.IsVisible = false;
            myCurve.Symbol.Fill = new Fill(Color.Blue);

            myCurve = myPane.AddCurve("Points", list1, Color.Gray, SymbolType.Circle);
            myCurve.Line.IsVisible = false;
            myCurve.Symbol.Border.IsVisible = false;
            myCurve.Symbol.Fill = new Fill(Color.Gray);

            myCurve = myPane.AddCurve("Simple", list2, Color.Red, SymbolType.Circle);
            myCurve.Line.IsAntiAlias = true;
            myCurve.Line.IsVisible = true;
            myCurve.Symbol.IsVisible = false;

            myCurve = myPane.AddCurve("RANSAC", list3, Color.Blue, SymbolType.Circle);
            myCurve.Line.IsAntiAlias = true;
            myCurve.Line.IsVisible = true;
            myCurve.Symbol.IsVisible = false;


            zgc.AxisChange();
            zgc.Invalidate();
        }


        #region Menus
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
                        this.dgvAnalysisSource.DataSource = tableSource;

                        double[,] data = tableSource.ToMatrix();
                        var x = data.GetColumn(0);
                        var y = data.GetColumn(1);
                        CreateScatterplot(graphInput, x, y, null, null, null, null);
                    }
                }
            }

            lbStatus.Text = "Click Compute the estimate a robust linear regression!";
        }
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }

    }
}
