﻿// Accord.NET Sample Applications
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

using Accord.Controls;
using Accord.IO;
using Accord.Math;
using Accord.Statistics;
using Accord.Statistics.Analysis;
using Accord.Statistics.Kernels;
using Components;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ZedGraph;

namespace SampleApp
{
    public partial class MainForm : Form
    {
        private KernelDiscriminantAnalysis kda;
        private DescriptiveAnalysis sda;

        string[] columnNames;
        double[][] inputs;
        int[] outputs;


        public MainForm()
        {
            InitializeComponent();

            dgvAnalysisSource.AutoGenerateColumns = true;
            dgvDistributionMeasures.AutoGenerateColumns = false;
            dgvFeatureVectors.AutoGenerateColumns = true;
            dgvPrincipalComponents.AutoGenerateColumns = false;
            dgvProjectionComponents.AutoGenerateColumns = false;
            dgvReversionComponents.AutoGenerateColumns = false;
            dgvProjectionResult.AutoGenerateColumns = true;

            openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Resources");
        }


        /// <summary>
        ///   Launched when the user clicks the File -> Open menu item.
        /// </summary>
        /// 
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
                        this.dgvProjectionSource.DataSource = tableSource.Copy();

                        double[][] graph = tableSource.ToJagged(out columnNames); 
                        inputScatterplot.DataSource = graph;
                        createMappingScatterplot(graphMapInput, graph);

                        lbStatus.Text = "Now, click 'Run analysis' to start processing the data!";
                    }
                }
            }
        }


        /// <summary>
        ///   Launched when the user clicks the "Run analysis" button.
        /// </summary>
        /// 
        private void btnCompute_Click(object sender, EventArgs e)
        {
            // Save any pending changes 
            dgvAnalysisSource.EndEdit();

            if (dgvAnalysisSource.DataSource == null)
            {
                MessageBox.Show("Please load some data using File > Open!");
                return;
            }

            // Creates a matrix from the source data table
            double[][] sourceMatrix = (dgvAnalysisSource.DataSource as DataTable).ToJagged(out columnNames);


            // Create and compute a new Simple Descriptive Analysis
            sda = new DescriptiveAnalysis(columnNames).Learn(sourceMatrix);

            // Show the descriptive analysis on the screen
            dgvDistributionMeasures.DataSource = sda.Measures;


            // Create the kernel function
            IKernel kernel = createKernel();

            // Get the input values (the two first columns)
            this.inputs = sourceMatrix.GetColumns(0, 1);

            // Get only the associated labels (last column)
            this.outputs = sourceMatrix.GetColumn(2).ToMulticlass();


            // Creates the Kernel Discriminant Analysis of the given data
            kda = new KernelDiscriminantAnalysis(kernel)
            {
                // Keep only the important components
                Threshold = (double)numThreshold.Value,
                NumberOfOutputs = 2 // use two components
            };

            // Use the analysis to create a classifier
            var classifier = kda.Learn(inputs, outputs);


            if (kda.Discriminants.Count < 2)
            {
                MessageBox.Show("Could not gather enough components to create"
                    + " a 2D plot. Please try a smaller threshold value.");
                return;
            }

            // Perform the transformation of the data
            double[][] result = kda.Transform(inputs);

            // Create a new plot with the original Z column
            double[][] points = result.InsertColumn(sourceMatrix.GetColumn(2));


            // Create output scatter plot
            outputScatterplot.DataSource = points;
            createMappingScatterplot(graphMapFeature, points);

            // Create output table
            dgvProjectionResult.DataSource = new ArrayDataView(points, columnNames);


            // Populates components overview with analysis data
            dgvFeatureVectors.DataSource = new ArrayDataView(kda.DiscriminantVectors.Transpose());
            dgvScatterBetween.DataSource = new ArrayDataView(kda.ScatterBetweenClass);
            dgvScatterWithin.DataSource = new ArrayDataView(kda.ScatterWithinClass);
            dgvScatterTotal.DataSource = new ArrayDataView(kda.ScatterMatrix);
            dgvPrincipalComponents.DataSource = kda.Discriminants;
            distributionView.DataSource = kda.Discriminants;
            cumulativeView.DataSource = kda.Discriminants;

            // Populates classes information
            dgvClasses.DataSource = kda.Classes;


            lbStatus.Text = "Good! Feel free to browse the other tabs to see what has been found.";
        }

        /// <summary>
        ///   Launched when the user clicks the "Compute projection" button.
        /// </summary>
        /// 
        private void btnProject_Click(object sender, EventArgs e)
        {
            if (kda == null || dgvProjectionSource.DataSource == null)
            {
                MessageBox.Show("Please compute the analysis first.");
                return;
            }

            // Save any pending changes 
            dgvProjectionSource.EndEdit();

            // Creates a matrix from the source data table
            double[][] sourceMatrix = (dgvProjectionSource.DataSource as DataTable).ToJagged(out columnNames);

            // Gets only the X and Y
            double[][] data = sourceMatrix.GetColumns(0, 1);

            // Perform the transformation of the data
            double[][] result = kda.Transform(data);

            // Create a new plot with the original Z column
            double[][] graph = result.InsertColumn(sourceMatrix.GetColumn(2));

            // Create output scatter plot
            outputScatterplot.DataSource = graph;
            createMappingScatterplot(graphMapFeature, graph);

            // Create output table
            dgvProjectionResult.DataSource = new ArrayDataView(graph, columnNames);
        }



        /// <summary>
        ///   Creates a kernel function with the user interface settings.
        /// </summary>
        /// 
        private IKernel createKernel()
        {
            if (rbGaussian.Checked)
                return new Gaussian((double)numSigma.Value);

            return new Polynomial((int)numDegree.Value, (double)numConstant.Value);
        }



        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {
            if (dgvDistributionMeasures.CurrentRow != null)
            {
                DataGridViewRow row = (DataGridViewRow)dgvDistributionMeasures.CurrentRow;
                DescriptiveMeasures measures = (DescriptiveMeasures)row.DataBoundItem;
                dataHistogramView1.DataSource = inputs.InsertColumn(outputs).GetColumn(measures.Index);
            }
        }

        private void dgvClasses_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgvClasses.CurrentRow != null)
            {
                DiscriminantAnalysisClass dclass = (DiscriminantAnalysisClass)dgvClasses.CurrentRow.DataBoundItem;
                dgvScatter.DataSource = new ArrayDataView(dclass.Scatter);
                dgvClassData.DataSource = new ArrayDataView(inputs.Get(outputs.Find(x => x == dclass.Number)));
            }
        }

        private void dgvClassData_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgvClassData.CurrentCell != null && dgvClassData.DataSource != null)
            {
                int index = dgvClassData.CurrentCell.ColumnIndex;
                double[][] subset = (double[][])(dgvClassData.DataSource as ArrayDataView).ArrayData;
                dataHistogramView2.DataSource = subset.GetColumn(index);
            }
        }


        /// <summary>
        ///   Creates the space mapping scatter plot using ZedGraph.
        /// </summary>
        /// 
        private void createMappingScatterplot(ZedGraphControl zgc, double[][] graph)
        {
            GraphPane myPane = zgc.GraphPane;
            myPane.CurveList.Clear();

            // Set the titles
            myPane.Title.Text = "Scatter Plot";
            myPane.XAxis.Title.Text = "X";
            myPane.YAxis.Title.Text = "Y";
            myPane.XAxis.IsAxisSegmentVisible = false;
            myPane.YAxis.IsAxisSegmentVisible = false;


            PointPairList list1 = new PointPairList();
            PointPairList list2 = new PointPairList();
            PointPairList list3 = new PointPairList();
            for (int i = 0; i < graph.GetLength(0); i++)
            {
                if (graph[i][2] == 1.0)
                    list1.Add(graph[i][0], graph[i][1]);
                if (graph[i][2] == 2.0)
                    list2.Add(graph[i][0], graph[i][1]);
                if (graph[i][2] == 3.0)
                    list3.Add(graph[i][0], graph[i][1]);
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

            myCurve = myPane.AddCurve("G3", list3, Color.Orange, SymbolType.Diamond);
            myCurve.Line.IsVisible = false;
            myCurve.Symbol.Border.IsVisible = false;
            myCurve.Symbol.Fill = new Fill(Color.Orange);

            myCurve = myPane.AddCurve("M1", new PointPairList(), Color.Blue, SymbolType.Diamond);
            myCurve.Line.IsVisible = false;
            myCurve.Symbol.Border.IsVisible = false;
            myCurve.Symbol.Fill = new Fill(Color.Blue);

            myCurve = myPane.AddCurve("M2", new PointPairList(), Color.Green, SymbolType.Diamond);
            myCurve.Line.IsVisible = false;
            myCurve.Symbol.Border.IsVisible = false;
            myCurve.Symbol.Fill = new Fill(Color.Green);

            myCurve = myPane.AddCurve("M3", new PointPairList(), Color.Orange, SymbolType.Diamond);
            myCurve.Line.IsVisible = false;
            myCurve.Symbol.Border.IsVisible = false;
            myCurve.Symbol.Fill = new Fill(Color.Orange);

            // Fill the background of the chart rect and pane
            myPane.Fill = new Fill(Color.WhiteSmoke);

            zgc.AxisChange();
            zgc.Invalidate();
        }


        private void graphMapInput_MouseMove(object sender, MouseEventArgs e)
        {
            if (kda == null)
                return;

            double x, y;
            graphMapInput.GraphPane.ReverseTransform(new PointF(e.X, e.Y), out x, out y);

            double[] result = kda.Transform(new double[] { x, y });

            int c = kda.Classifier.Decide(new double[] { x, y });

            graphMapFeature.GraphPane.CurveList["M1"].Clear();
            graphMapFeature.GraphPane.CurveList["M2"].Clear();
            graphMapFeature.GraphPane.CurveList["M3"].Clear();

            if (c == 0)
                graphMapFeature.GraphPane.CurveList["M1"].AddPoint(result[0], result[1]);
            else if (c == 1)
                graphMapFeature.GraphPane.CurveList["M2"].AddPoint(result[0], result[1]);
            else
                graphMapFeature.GraphPane.CurveList["M3"].AddPoint(result[0], result[1]);

            graphMapFeature.Invalidate();

        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Create a matrix from the source data table
            double[,] sourceMatrix = (dgvAnalysisSource.DataSource as DataTable).ToMatrix(out columnNames);

            // Get the input values (the two first columns)
            double[,] inputs = sourceMatrix.GetColumns(0, 1);

            numSigma.Value = (decimal)Gaussian.Estimate(inputs.ToJagged()).Sigma;
        }

        private void splitContainer12_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

    }
}
