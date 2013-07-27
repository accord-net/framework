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
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using Accord.Statistics.Analysis;
using Accord.Math;

using System.IO;
using ZedGraph;

using Components;
using Accord.Controls;
using Accord.Statistics.Formats;

using System.Linq;


namespace Discriminant
{
    public partial class MainForm : Form
    {


        // Colors used in the pie graphics.
        private readonly Color[] colors = { Color.YellowGreen, Color.DarkOliveGreen, Color.DarkKhaki, Color.Olive,
            Color.Honeydew, Color.PaleGoldenrod, Color.Indigo, Color.Olive, Color.SeaGreen };


        private LinearDiscriminantAnalysis lda;
        private DescriptiveAnalysis sda;

        string[] sourceColumns;


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

                        double[,] graph = tableSource.ToMatrix(out sourceColumns);
                        inputScatterplot.DataSource = graph;
                    }
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // Finishes and save any pending changes to the given data
            dgvAnalysisSource.EndEdit();

            if (dgvAnalysisSource.DataSource == null) return;

            // Creates a matrix from the source data table
            double[,] sourceMatrix = (dgvAnalysisSource.DataSource as DataTable).ToMatrix(out sourceColumns);

            int rows = sourceMatrix.GetLength(0);
            int cols = sourceMatrix.GetLength(1);


            // Creates a new Simple Descriptive Analysis
            sda = new DescriptiveAnalysis(sourceMatrix, sourceColumns);

            sda.Compute();

            // Populates statistics overview tab with analysis data
            dgvDistributionMeasures.DataSource = sda.Measures;


            // Get only the input values (exclude the class label indicator column)
            double[,] data = sourceMatrix.Submatrix(null, startColumn: 0, endColumn: 1);

            // Get only the associated labels
            int[] labels = sourceMatrix.GetColumn(2).ToInt32();


            // Creates the Linear Discriminant Analysis of the given source
            lda = new LinearDiscriminantAnalysis(data, labels);


            // Computes the analysis
            lda.Compute();


            // Performs the transformation of the data using two dimensions
            double[,] result = lda.Transform(data, 2);

            // Create a new plot with the original Z column
            double[,] points = result.InsertColumn(sourceMatrix.GetColumn(2));


            // Create output scatter plot
            outputScatterplot.DataSource = points;

            // Create output table
            dgvProjectionResult.DataSource = new ArrayDataView(points, sourceColumns);


            // Populates discriminants overview with analysis data
            dgvPrincipalComponents.DataSource = lda.Discriminants;
            dgvFeatureVectors.DataSource = new ArrayDataView(lda.DiscriminantMatrix);
            dgvScatterBetween.DataSource = new ArrayDataView(lda.ScatterBetweenClass);
            dgvScatterWithin.DataSource = new ArrayDataView(lda.ScatterWithinClass);
            dgvScatterTotal.DataSource = new ArrayDataView(lda.ScatterMatrix);


            // Populates classes information
            dgvClasses.DataSource = lda.Classes;


            CreateComponentCumulativeDistributionGraph(graphCurve);
            CreateComponentDistributionGraph(graphShare);

        }

        private void btnProject2_Click(object sender, EventArgs e)
        {
            if (lda == null)
            {
                MessageBox.Show("Please run the analysis first!");
                return;
            }

            // Finishes and save any pending changes to the given data
            dgvProjectionSource.EndEdit();

            // Creates a matrix from the source data table
            double[,] sourceMatrix = (dgvProjectionSource.DataSource as DataTable).ToMatrix(out sourceColumns);

            // Gets only the X and Y
            double[,] data = sourceMatrix.Submatrix(null, 0, 1);

            // Perform the transformation of the data using two components
            double[,] result = lda.Transform(data, 2);

            // Create a new plot with the original Z column
            double[,] graph = new double[sourceMatrix.GetLength(0), 3];
            for (int i = 0; i < graph.GetLength(0); i++)
            {
                graph[i, 0] = result[i, 0];
                graph[i, 1] = result[i, 1];
                graph[i, 2] = sourceMatrix[i, 2];
            }

            // Create output scatter plot
            //CreateScatterplot(graphOutput, graph);
            outputScatterplot.DataSource = graph;

            // Create output table
            dgvProjectionResult.DataSource = new ArrayDataView(graph, sourceColumns);
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

        private void dgvClasses_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgvClasses.CurrentRow != null)
            {
                DiscriminantAnalysisClass dclass = (DiscriminantAnalysisClass)dgvClasses.CurrentRow.DataBoundItem;

                dgvScatter.DataSource = new ArrayDataView(dclass.Scatter);
                dgvClassData.DataSource = new ArrayDataView(dclass.Subset);
            }
        }

        private void dgvClassData_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgvClassData.CurrentCell != null && dgvClassData.DataSource != null)
            {
                int index = dgvClassData.CurrentCell.ColumnIndex;
                double[,] subset = (double[,])(dgvClassData.DataSource as ArrayDataView).ArrayData;
                dataHistogramView2.DataSource = subset.GetColumn(index);
            }
        }

        public void CreateComponentCumulativeDistributionGraph(ZedGraphControl zgc)
        {
            GraphPane myPane = zgc.GraphPane;

            myPane.CurveList.Clear();

            // Set the titles and axis labels
            myPane.Title.Text = "Component Distribution";
            myPane.Title.FontSpec.Size = 24f;
            myPane.Title.FontSpec.Family = "Tahoma";
            myPane.XAxis.Title.Text = "Components";
            myPane.YAxis.Title.Text = "Percentage";

            PointPairList list = new PointPairList();
            for (int i = 0; i < lda.Discriminants.Count; i++)
            {
                list.Add(lda.Discriminants[i].Index, lda.Discriminants[i].CumulativeProportion);
            }

            // Hide the legend
            myPane.Legend.IsVisible = false;

            // Add a curve
            LineItem curve = myPane.AddCurve("label", list, Color.Red, SymbolType.Circle);
            curve.Line.Width = 2.0F;
            curve.Line.IsAntiAlias = true;
            curve.Symbol.Fill = new Fill(Color.White);
            curve.Symbol.Size = 7;

            myPane.XAxis.Scale.MinAuto = true;
            myPane.XAxis.Scale.MaxAuto = true;
            myPane.YAxis.Scale.MinAuto = true;
            myPane.YAxis.Scale.MaxAuto = true;
            myPane.XAxis.Scale.MagAuto = true;
            myPane.YAxis.Scale.MagAuto = true;


            // Calculate the Axis Scale Ranges
            zgc.AxisChange();
            zgc.Invalidate();
        }

        public void CreateComponentDistributionGraph(ZedGraphControl zgc)
        {
            GraphPane myPane = zgc.GraphPane;
            myPane.CurveList.Clear();

            // Set the GraphPane title
            myPane.Title.Text = "Component Proportion";
            myPane.Title.FontSpec.Size = 24f;
            myPane.Title.FontSpec.Family = "Tahoma";

            // Fill the pane background with a color gradient
            //  myPane.Fill = new Fill(Color.White, Color.WhiteSmoke, 45.0f);
            // No fill for the chart background
            myPane.Chart.Fill.Type = FillType.None;

            myPane.Legend.IsVisible = false;

            // Add some pie slices
            for (int i = 0; i < lda.Discriminants.Count; i++)
            {
                myPane.AddPieSlice(lda.Discriminants[i].Proportion, colors[i % colors.Length], 0.1, lda.Discriminants[i].Index.ToString());
            }


            myPane.XAxis.Scale.MinAuto = true;
            myPane.XAxis.Scale.MaxAuto = true;
            myPane.YAxis.Scale.MinAuto = true;
            myPane.YAxis.Scale.MaxAuto = true;
            myPane.XAxis.Scale.MagAuto = true;
            myPane.YAxis.Scale.MagAuto = true;


            // Calculate the Axis Scale Ranges
            zgc.AxisChange();

            zgc.Invalidate();
        }


    }
}
