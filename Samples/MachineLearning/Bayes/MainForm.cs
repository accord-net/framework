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

using Accord.Controls;
using Accord.MachineLearning.Bayes;
using Accord.Math;
using Accord.Statistics.Analysis;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Formats;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ZedGraph;

namespace Bayes
{
    public partial class MainForm : Form
    {

        private NaiveBayes<NormalDistribution> bayes;

        string[] sourceColumns;
        string[] sourceClasses;


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

            sourceClasses = new string[] { "G1", "G2" };


            // Finishes and save any pending changes to the given data
            dgvLearningSource.EndEdit();

            // Creates a matrix from the source data table
            double[,] sourceMatrix = (dgvLearningSource.DataSource as DataTable).ToMatrix(out sourceColumns);

            // Get only the input vector values
            double[][] inputs = sourceMatrix.Submatrix(null, 0, 1).ToArray();

            // Get only the label outputs
            int[] outputs = sourceMatrix.GetColumn(2).ToInt32();
            string[] colNames = sourceColumns.Submatrix(first: 2);


            // Create the Bayes classifier and perform classification
            bayes = new NaiveBayes<NormalDistribution>(2, 2, new NormalDistribution());
            double error = bayes.Estimate(inputs, outputs);


            // Show the estimated distributions and class probabilities
            dataGridView1.DataSource = new ArrayDataView(bayes.Distributions, colNames, sourceClasses);


            // Generate samples for class 1
            var x1 = bayes.Distributions[0, 0].Generate(1000);
            var y1 = bayes.Distributions[0, 1].Generate(1000);

            // Generate samples for class 2
            var x2 = bayes.Distributions[1, 0].Generate(1000);
            var y2 = bayes.Distributions[1, 1].Generate(1000);

            // Combine in a single graph
            var w1 = Matrix.Stack(x1, y1).Transpose();
            var w2 = Matrix.Stack(x2, y2).Transpose();

            var z = Matrix.Vector(2000, value: 1.0);
            for (int i = 0; i < 1000; i++) z[i] = 0;

            var graph = Matrix.Stack(w1, w2).Concatenate(z);

            CreateScatterplot(zedGraphControl2, graph);
        }


        private void btnTestingRun_Click(object sender, EventArgs e)
        {
            if (bayes == null || dgvTestingSource.DataSource == null)
            {
                MessageBox.Show("Please create a machine first.");
                return;
            }


            // Creates a matrix from the source data table
            double[,] sourceMatrix = (dgvLearningSource.DataSource as DataTable).ToMatrix();


            // Get only the input vector values
            double[][] inputs = sourceMatrix.Submatrix(null, 0, 1).ToArray();

            // Get only the label outputs
            int[] expected = new int[sourceMatrix.GetLength(0)];
            for (int i = 0; i < expected.Length; i++)
                expected[i] = (int)sourceMatrix[i, 2];

            // Compute the machine outputs
            int[] output = new int[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
                output[i] = bayes.Compute(inputs[i]);


            // Use confusion matrix to compute some statistics.
            ConfusionMatrix confusionMatrix = new ConfusionMatrix(output, expected, 1, 0);
            dgvPerformance.DataSource = new List<ConfusionMatrix> { confusionMatrix };

            foreach (DataGridViewColumn col in dgvPerformance.Columns)
                col.Visible = true;
            Column1.Visible = Column2.Visible = false;

            // Create performance scatterplot
            CreateResultScatterplot(zedGraphControl1, inputs, expected.ToDouble(), output.ToDouble());
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
            PointPairList list1 = new PointPairList(); // Z = 0
            PointPairList list2 = new PointPairList(); // Z = 1
            for (int i = 0; i < graph.GetLength(0); i++)
            {
                if (graph[i, 2] == 0)
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
            PointPairList list1 = new PointPairList(); // Z = 0, OK
            PointPairList list2 = new PointPairList(); // Z = 1, OK
            PointPairList list3 = new PointPairList(); // Z = 0, Error
            PointPairList list4 = new PointPairList(); // Z = 1, Error
            for (int i = 0; i < output.Length; i++)
            {
                if (output[i] == 0)
                {
                    if (expected[i] == 0)
                        list1.Add(inputs[i][0], inputs[i][1]);
                    if (expected[i] == 1)
                        list3.Add(inputs[i][0], inputs[i][1]);
                }
                else
                {
                    if (expected[i] == 0)
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


        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                dataGridView1.Rows[i].HeaderCell.Value = sourceClasses[i];

            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
        }

    }
}
