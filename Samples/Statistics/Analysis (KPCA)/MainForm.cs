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
using Accord.Controls;
using Accord.Math;
using Accord.Statistics.Analysis;
using Accord.Statistics.Formats;
using Accord.Statistics.Kernels;
using Components;
using ZedGraph;

namespace Analysis.KPCA
{
    public partial class MainForm : Form
    {
        /*
            Best values for the given examples
         
              Curve     = Correlation,    Center, Gaussian, sigma = 20
              Lindsay   = Covariance,     Center, Polynomial, degree = 1
              Linear    = Covariance,     Center, Polynomial, degree = 1
              Schopholf = Covariance,     Center, Gaussian, sigma = 0.2236
              Wikipedia = Covariance,  No center, Gaussian, sigma = 10
              Wikipedia = Covariance,  No center, polynomial, degree = 2
              Wikipedia = Correlation, No center, polynomial, degree = 2
          
         */

        private KernelPrincipalComponentAnalysis kpca;
        private DescriptiveAnalysis sda;

        string[] columnNames;


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

        private void MainForm_Load(object sender, EventArgs e)
        {
            Array methods = Enum.GetValues(typeof(AnalysisMethod));
            this.tscbMethod.ComboBox.DataSource = methods;
            this.cbMethod.DataSource = methods;
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

                        double[,] graph = tableSource.ToMatrix(out columnNames);
                        inputScatterplot.DataSource = graph;
                        CreateScatterplot(graphMapInput, graph);

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

            // Create a matrix from the source data table
            double[,] sourceMatrix = (dgvAnalysisSource.DataSource as DataTable).ToMatrix(out columnNames);

            int rows = sourceMatrix.GetLength(0);
            int cols = sourceMatrix.GetLength(1);


            // Create and compute a new Simple Descriptive Analysis
            sda = new DescriptiveAnalysis(sourceMatrix, columnNames);

            sda.Compute();

            // Show the descriptive analysis on the screen
            dgvDistributionMeasures.DataSource = sda.Measures;


            // Create the kernel function
            IKernel kernel = createKernel();


            // Get the input values (the two first columns)
            double[,] inputs = sourceMatrix.GetColumns(0, 1);

            // Get only the associated labels (last column)
            int[] outputs = sourceMatrix.GetColumn(2).ToInt32();


            AnalysisMethod method = (AnalysisMethod)cbMethod.SelectedValue;

            // Creates the Kernel Principal Component Analysis of the given source
            kpca = new KernelPrincipalComponentAnalysis(inputs, kernel, method);

            // Whether to center in space
            kpca.Center = cbCenter.Checked;


            kpca.Compute(); // Finally, compute the analysis!


            
            double[,] result;

            if (kpca.Components.Count >= 2)
            {
                // Perform the transformation of the data using two components 
                result = kpca.Transform(inputs, 2);
            }
            else
            {
                result = kpca.Transform(inputs, 1);
                result = result.InsertColumn(Matrix.Vector(result.GetLength(0), 0.0));
            }

            // Create a new plot with the original Z column
            double[,] points = result.InsertColumn(sourceMatrix.GetColumn(2));

            // Create output scatter plot
            outputScatterplot.DataSource = points;
            CreateScatterplot(graphMapFeature, points);

            // Create output table
            dgvProjectionResult.DataSource = new ArrayDataView(points, columnNames);
            dgvReversionSource.DataSource = new ArrayDataView(kpca.Result);


            // Populates components overview with analysis data
            dgvFeatureVectors.DataSource = new ArrayDataView(kpca.ComponentMatrix);
            dgvPrincipalComponents.DataSource = kpca.Components;
            dgvProjectionComponents.DataSource = kpca.Components;
            dgvReversionComponents.DataSource = kpca.Components;
            cumulativeView.DataSource = kpca.Components;
            distributionView.DataSource = kpca.Components;

            numComponents.Maximum = kpca.Components.Count;
            numNeighbor.Maximum = kpca.Result.GetLength(0);
            numNeighbor.Value = System.Math.Min(10, numNeighbor.Maximum);

            lbStatus.Text = "Good! Feel free to browse the other tabs to see what has been found.";
        }

        /// <summary>
        ///   Launched when the user clicks the "Compute projection" button.
        /// </summary>
        /// 
        private void btnProject_Click(object sender, EventArgs e)
        {
            // Save any pending changes 
            dgvProjectionSource.EndEdit();

            // Creates a matrix from the source data table
            double[,] sourceMatrix = (dgvProjectionSource.DataSource as DataTable).ToMatrix(out columnNames);

            // Gets only the X and Y
            double[,] data = sourceMatrix.Submatrix(0, sourceMatrix.GetLength(0) - 1, 0, 1);

            // Perform the transformation of the data using two components
            double[,] result = kpca.Transform(data, 2);

            // Create a new plot with the original Z column
            double[,] graph = new double[sourceMatrix.GetLength(0), 3];
            for (int i = 0; i < graph.GetLength(0); i++)
            {
                graph[i, 0] = result[i, 0];
                graph[i, 1] = result[i, 1];
                graph[i, 2] = sourceMatrix[i, 2];
            }

            // Create output scatter plot
            outputScatterplot.DataSource = graph;
            CreateScatterplot(graphMapFeature, graph);

            // Create output table
            dgvProjectionResult.DataSource = new ArrayDataView(graph);
        }

        private void btnReversion_Click(object sender, EventArgs e)
        {
            double[,] reversionSource = (double[,])(dgvReversionSource.DataSource as ArrayDataView).ArrayData;
            double[,] m = kpca.Revert(reversionSource, (int)numNeighbor.Value);
            dgvReversionResult.DataSource = new ArrayDataView(m);

            // Creates a matrix from the source data table
            double[,] sourceMatrix = (dgvProjectionSource.DataSource as DataTable).ToMatrix();


            // Create a new plot with the original Z column
            double[,] graph = new double[sourceMatrix.GetLength(0), 3];
            for (int i = 0; i < graph.GetLength(0); i++)
            {
                graph[i, 0] = m[i, 0];
                graph[i, 1] = m[i, 1];
                graph[i, 2] = sourceMatrix[i, 2];
            }

            reversionScatterplot.DataSource = graph;
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
                dataHistogramView1.DataSource =
                    ((DescriptiveMeasures)row.DataBoundItem).Samples;
            }
        }



        public void CreateScatterplot(ZedGraphControl zgc, double[,] graph)
        {
            GraphPane myPane = zgc.GraphPane;
            myPane.CurveList.Clear();

            // Set the titles
            myPane.Title.Text = "Scatter Plot";
            myPane.XAxis.Title.Text = "X";
            myPane.YAxis.Title.Text = "Y";


            PointPairList list1 = new PointPairList();
            PointPairList list2 = new PointPairList();
            PointPairList list3 = new PointPairList();

            for (int i = 0; i < graph.GetLength(0); i++)
            {
                double x = graph[i, 0], y = graph[i, 1], z = graph[i, 2];

                if (z == 1.0) list1.Add(x, y);
                if (z == 2.0) list2.Add(x, y);
                if (z == 3.0) list3.Add(x, y);
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

            myCurve = myPane.AddCurve("M", new PointPairList(), Color.Black, SymbolType.Diamond);
            myCurve.Line.IsVisible = false;
            myCurve.Symbol.Border.IsVisible = false;
            myCurve.Symbol.Fill = new Fill(Color.Black);

            // Fill the background of the chart rect and pane
            //myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45.0f);
            //myPane.Fill = new Fill(Color.White, Color.SlateGray, 45.0f);
            myPane.Fill = new Fill(Color.WhiteSmoke);

            zgc.AxisChange();
            zgc.Invalidate();
        }


        private void graphMapInput_MouseMove(object sender, MouseEventArgs e)
        {
            double x;
            double y;
            graphMapInput.GraphPane.ReverseTransform(new PointF(e.X, e.Y), out x, out y);

            double[,] data = new double[1, 2];
            data[0, 0] = x;
            data[0, 1] = y;


            double[,] result = kpca.Transform(data);

            graphMapFeature.GraphPane.CurveList["M"].Clear();
            graphMapFeature.GraphPane.CurveList["M"].AddPoint(result[0, 0], result[0, 1]);
            graphMapFeature.Invalidate();
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
