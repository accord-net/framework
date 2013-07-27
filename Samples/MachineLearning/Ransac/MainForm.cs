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

namespace Ransac
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();

            openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Resources");
        }



        private void btnSampleRunAnalysis_Click(object sender, EventArgs e)
        {
            DataTable dataTable = dgvAnalysisSource.DataSource as DataTable;

            if (dataTable == null) return;

            // Gather the available data
            double[][] data = dataTable.ToArray();


            // First, fit simple linear regression directly for comparison reasons.
            double[] x = data.GetColumn(0); // Extract the independent variable
            double[] y = data.GetColumn(1); // Extract the dependent variable

            // Create a simple linear regression
            SimpleLinearRegression slr = new SimpleLinearRegression();
            slr.Regress(x, y);

            // Compute the simple linear regression output
            double[] slrY = slr.Compute(x);


            // Now, fit simple linear regression using RANSAC
            int maxTrials = (int)numMaxTrials.Value;
            int minSamples = (int)numSamples.Value;
            double probability = (double)numProbability.Value;
            double errorThreshold = (double)numThreshold.Value;

            // Create a RANSAC algorithm to fit a simple linear regression
            var ransac = new RANSAC<SimpleLinearRegression>(minSamples);
            ransac.Probability = probability;
            ransac.Threshold = errorThreshold;
            ransac.MaxEvaluations = maxTrials;

            // Set the RANSAC functions to evaluate and test the model

            ransac.Fitting = // Define a fitting function
                delegate(int[] sample)
                {
                    // Retrieve the training data
                    double[] inputs = x.Submatrix(sample);
                    double[] outputs = y.Submatrix(sample);

                    // Build a Simple Linear Regression model
                    var r = new SimpleLinearRegression();
                    r.Regress(inputs, outputs);
                    return r;
                };

            ransac.Degenerate = // Define a check for degenerate samples
                delegate(int[] sample)
                {
                    // In this case, we will not be performing such checks.
                    return false;
                };

            ransac.Distances = // Define a inlier detector function
                delegate(SimpleLinearRegression r, double threshold)
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
                };


            // Finally, try to fit the regression model using RANSAC
            int[] idx; SimpleLinearRegression rlr = ransac.Compute(data.Length, out idx);



            // Check if RANSAC was able to build a consistent model
            if (rlr == null)
            {
                return; // RANSAC was unsucessful, just return.
            }
            else
            {
                // Compute the output of the model fitted by RANSAC
                double[] rlrY = rlr.Compute(x);

                // Create scatterplot comparing the outputs from the standard
                //  linear regression and the RANSAC-fitted linear regression.
                CreateScatterplot(graphInput, x, y, slrY, rlrY, x.Submatrix(idx), y.Submatrix(idx));
            }
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
        }
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

    }
}
