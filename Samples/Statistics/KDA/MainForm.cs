using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using Accord.Statistics.Analysis;
using Accord.Statistics.Kernels;
using Accord.Math;

using System.IO;
using ZedGraph;

using Accord.Controls;
using Components;
using Accord.Statistics.Formats;


namespace KernelDiscriminant
{
    public partial class MainForm : Form
    {


        // Colors used in the pie graphics.
        private readonly Color[] colors = { Color.YellowGreen, Color.DarkOliveGreen, Color.DarkKhaki, Color.Olive,
            Color.Honeydew, Color.PaleGoldenrod, Color.Indigo, Color.Olive, Color.SeaGreen };


        private KernelDiscriminantAnalysis kda;
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
                        CreateScatterplot(graphMapInput, graph);
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


            // Creates a new Simple Descriptive Analysis
            sda = new DescriptiveAnalysis(sourceMatrix, sourceColumns);
            sda.Compute();
            dgvDistributionMeasures.DataSource = sda.Measures;


            // Populates statistics overview tab with analysis data
            dgvDistributionMeasures.DataSource = sda.Measures;


            IKernel kernel;
            if (rbGaussian.Checked)
            {
                kernel = new Gaussian((double)numSigma.Value);
            }
            else
            {
                kernel = new Polynomial((int)numDegree.Value, (double)numConstant.Value);
            }


            // Get only the input values (exclude the class label indicator column)
            double[,] data = sourceMatrix.Submatrix(null, startColumn: 0, endColumn: 1);

            // Get only the associated labels
            int[] labels = sourceMatrix.GetColumn(2).ToInt32();


            // Creates the Kernel Discriminant Analysis of the given source
            kda = new KernelDiscriminantAnalysis(data, labels, kernel);

            // Keep all components
            kda.Threshold = (double)numThreshold.Value;


            // Computes the analysis
            kda.Compute();


            // Perform the transformation of the data using two components
            double[,] result = kda.Transform(data, 2);

            // Create a new plot with the original Z column
            double[,] points = result.InsertColumn(sourceMatrix.GetColumn(2));


            // Create output scatter plot
            outputScatterplot.DataSource = points;
            CreateScatterplot(graphMapFeature, points);

            // Create output table
            dgvProjectionResult.DataSource = new ArrayDataView(points, sourceColumns);


            // Populates components overview with analysis data
            dgvFeatureVectors.DataSource = new ArrayDataView(kda.DiscriminantMatrix);
            dgvPrincipalComponents.DataSource = kda.Discriminants;
            dgvScatterBetween.DataSource = new ArrayDataView(kda.ScatterBetweenClass);
            dgvScatterWithin.DataSource = new ArrayDataView(kda.ScatterWithinClass);
            dgvScatterTotal.DataSource = new ArrayDataView(kda.ScatterMatrix);

            // Populates classes information
            dgvClasses.DataSource = kda.Classes;


            CreateComponentCumulativeDistributionGraph(graphCurve);
            CreateComponentDistributionGraph(graphShare);

        }

        private void btnProject2_Click(object sender, EventArgs e)
        {
            if (kda == null || dgvProjectionSource.DataSource == null)
            {
                MessageBox.Show("Please compute the analysis first.");
                return;
            }

            // Finishes and save any pending changes to the given data
            dgvProjectionSource.EndEdit();

            // Creates a matrix from the source data table
            double[,] sourceMatrix = (dgvProjectionSource.DataSource as DataTable).ToMatrix(out sourceColumns);

            // Gets only the X and Y
            double[,] data = sourceMatrix.Submatrix(0, sourceMatrix.GetLength(0) - 1, 0, 1);

            // Perform the transformation of the data using two components
            double[,] result = kda.Transform(data, 2);

            // Classification
            //int[] output = kda.Classify(data.ToArray());

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
            for (int i = 0; i < kda.Discriminants.Count; i++)
            {
                list.Add(kda.Discriminants[i].Index, kda.Discriminants[i].CumulativeProportion);
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
            //myPane.Fill = new Fill(Color.White);

            // No fill for the chart background
            myPane.Chart.Fill.Type = FillType.None;

            myPane.Legend.IsVisible = false;

            // Add some pie slices
            for (int i = 0; i < kda.Discriminants.Count; i++)
            {
                myPane.AddPieSlice(kda.Discriminants[i].Proportion, colors[i % colors.Length], 0.1, kda.Discriminants[i].Index.ToString());
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


        public void CreateScatterplot(ZedGraphControl zgc, double[,] graph)
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
                if (graph[i, 2] == 1.0)
                    list1.Add(graph[i, 0], graph[i, 1]);
                if (graph[i, 2] == 2.0)
                    list2.Add(graph[i, 0], graph[i, 1]);
                if (graph[i, 2] == 3.0)
                    list3.Add(graph[i, 0], graph[i, 1]);
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
            myCurve.Symbol.Fill = new Fill(Color.Yellow);

            // Fill the background of the chart rect and pane
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


            double[,] result = kda.Transform(data);

            int c = kda.Classify(new double[] { x, y });

            graphMapFeature.GraphPane.CurveList["M1"].Clear();
            graphMapFeature.GraphPane.CurveList["M2"].Clear();
            graphMapFeature.GraphPane.CurveList["M3"].Clear();

            if (c == 1)
                graphMapFeature.GraphPane.CurveList["M1"].AddPoint(result[0, 0], result[0, 1]);
            else if (c == 2)
                graphMapFeature.GraphPane.CurveList["M2"].AddPoint(result[0, 0], result[0, 1]);
            else
                graphMapFeature.GraphPane.CurveList["M3"].AddPoint(result[0, 0], result[0, 1]);

            graphMapFeature.Invalidate();

        }

    }
}
