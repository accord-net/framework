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
using Accord.Math;
using Accord.Statistics.Analysis;
using Accord.Statistics.Kernels;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using ZedGraph;

namespace Handwriting
{
    public partial class MainForm : Form
    {
        // Colors used in the pie graphics.
        private readonly Color[] colors = { Color.YellowGreen, Color.DarkOliveGreen, Color.DarkKhaki, Color.Olive,
            Color.Honeydew, Color.PaleGoldenrod, Color.Indigo, Color.Olive, Color.SeaGreen };


        KernelDiscriminantAnalysis kda;


        public MainForm()
        {
            InitializeComponent();
        }


        #region Data extraction
        private Bitmap Extract(string text)
        {
            Bitmap bitmap = new Bitmap(32, 32, PixelFormat.Format32bppRgb);
            string[] lines = text.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    if (lines[i][j] == '0')
                        bitmap.SetPixel(j, i, Color.White);
                    else bitmap.SetPixel(j, i, Color.Black);
                }
            }
            return bitmap;
        }

        private double[] Extract(Bitmap bmp)
        {
            double[] features = new double[32 * 32];
            for (int i = 0; i < 32; i++)
                for (int j = 0; j < 32; j++)
                    features[i * 32 + j] = (bmp.GetPixel(j, i).R == 255) ? 0 : 1;

            return features;
        }

        private double[] Preprocess(Bitmap bitmap)
        {
            double[] features = new double[64];

            for (int m = 0; m < 8; m++)
            {
                for (int n = 0; n < 8; n++)
                {
                    int c = m * 8 + n;
                    for (int i = m * 4; i < m * 4 + 4; i++)
                    {
                        for (int j = n * 4; j < n * 4 + 4; j++)
                        {
                            Color pixel = bitmap.GetPixel(j, i);
                            if (pixel.R == 0x00) // white
                                features[c] += 1;
                        }
                    }
                }
            }

            return features;
        }
        #endregion


        #region Form Events
        private void MainForm_Load(object sender, EventArgs e)
        {
            lbStatus.Text = "Click File->Open to load data.";
        }

        private void btnRunAnalysis_Click(object sender, EventArgs e)
        {
            if (dgvAnalysisSource.Rows.Count == 0)
            {
                MessageBox.Show("Please load the training data before clicking this button");
                return;
            }

            lbStatus.Text = "Gathering data. This may take a while...";
            Application.DoEvents();



            // Extract inputs and outputs
            int rows = dgvAnalysisSource.Rows.Count;
            double[,] input = new double[rows, 32 * 32];
            int[] output = new int[rows];
            for (int i = 0; i < rows; i++)
            {
                input.SetRow(i, (double[])dgvAnalysisSource.Rows[i].Cells["colTrainingFeatures"].Value);
                output[i] = (int)dgvAnalysisSource.Rows[i].Cells["colTrainingLabel"].Value;
            }

            // Create the chosen Kernel with given parameters
            IKernel kernel;
            if (rbGaussian.Checked)
                kernel = new Gaussian((double)numSigma.Value);
            else
                kernel = new Polynomial((int)numDegree.Value, (double)numConstant.Value);

            // Create the Kernel Discriminant Analysis using the selected Kernel
            kda = new KernelDiscriminantAnalysis(input, output, kernel);

            kda.Threshold = (double)numThreshold.Value;
            kda.Regularization = (double)numRegularization.Value;


            lbStatus.Text = "Computing the analysis. This may take a significant amount of time...";
            Application.DoEvents();

            // Compute the analysis. It should take a while.
            kda.Compute();


            // Show information about the analysis in the form
            dgvPrincipalComponents.DataSource = kda.Discriminants;
            dgvFeatureVectors.DataSource = new ArrayDataView(kda.DiscriminantMatrix);
            dgvClasses.DataSource = kda.Classes;

            // Create the component graphs
            CreateComponentCumulativeDistributionGraph(graphCurve);
            CreateComponentDistributionGraph(graphShare);

            lbStatus.Text = "Analysis complete. Click Classify to test the analysis.";

            btnClassify.Enabled = true;
        }

        private void btnClassify_Click(object sender, EventArgs e)
        {
            if (dgvAnalysisTesting.Rows.Count == 0)
            {
                MessageBox.Show("Please load the testing data before clicking this button");
                return;
            }
            else if (kda == null)
            {
                MessageBox.Show("Please perform the analysis before attempting classification");
                return;
            }

            lbStatus.Text = "Classification started. This may take a while...";
            Application.DoEvents();

            int hits = 0;
            progressBar.Visible = true;
            progressBar.Value = 0;
            progressBar.Step = 1;
            progressBar.Maximum = dgvAnalysisTesting.Rows.Count;

            // Extract inputs
            foreach (DataGridViewRow row in dgvAnalysisTesting.Rows)
            {
                double[] input = (double[])row.Cells["colTestingFeatures"].Value;
                int expected = (int)row.Cells["colTestingExpected"].Value;

                int output = kda.Classify(input);
                row.Cells["colTestingOutput"].Value = output;

                if (expected == output)
                {
                    row.Cells[0].Style.BackColor = Color.LightGreen;
                    row.Cells[1].Style.BackColor = Color.LightGreen;
                    row.Cells[2].Style.BackColor = Color.LightGreen;
                    hits++;
                }
                else
                {
                    row.Cells[0].Style.BackColor = Color.White;
                    row.Cells[1].Style.BackColor = Color.White;
                    row.Cells[2].Style.BackColor = Color.White;
                }

                progressBar.PerformStep();
            }

            progressBar.Visible = false;

            lbStatus.Text = String.Format("Classification complete. Hits: {0}/{1} ({2:0%})",
                hits, dgvAnalysisTesting.Rows.Count, (double)hits / dgvAnalysisTesting.Rows.Count);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            lbStatus.Text = "Loading data. This may take a while...";
            Application.DoEvents();


            // Load optdigits dataset into the DataGridView
            StringReader reader = new StringReader(Properties.Resources.optdigits_tra);

            int trainingStart = 0;
            int trainingCount = 500;

            int testingStart = 1000;
            int testingCount = 500;

            dgvAnalysisSource.Rows.Clear();
            dgvAnalysisTesting.Rows.Clear();

            int c = 0;
            while (true)
            {
                char[] buffer = new char[(32 + 2) * 32];
                int read = reader.ReadBlock(buffer, 0, buffer.Length);
                string label = reader.ReadLine();


                if (read < buffer.Length || label == null) break;

                if (c > trainingStart && c <= trainingStart + trainingCount)
                {
                    Bitmap bitmap = Extract(new String(buffer));
                    double[] features = Extract(bitmap);
                    int clabel = Int32.Parse(label);
                    dgvAnalysisSource.Rows.Add(bitmap, clabel, features);
                }
                else if (c > testingStart && c <= testingStart + testingCount)
                {
                    Bitmap bitmap = Extract(new String(buffer));
                    double[] features = Extract(bitmap);
                    int clabel = Int32.Parse(label);
                    dgvAnalysisTesting.Rows.Add(bitmap, clabel, null, features);
                }

                c++;
            }

            lbStatus.Text = String.Format(
                "Dataset loaded. Click Run analysis to start the analysis.",
                trainingCount, testingCount);

            btnSampleRunAnalysis.Enabled = true;
        }

        private void btnCanvasClassify_Click(object sender, EventArgs e)
        {
            if (kda != null)
            {
                // Get the input vector drawn
                double[] input = canvas.GetDigit();
                
                // Classify the input vector
                double[] responses;
                int num = kda.Classify(input, out responses);

                // Set the actual classification answer 
                lbCanvasClassification.Text = num.ToString();


                // Scale the responses to a [0,1] interval
                double max = responses.Max();
                double min = responses.Min();

                for (int i = 0; i < responses.Length; i++)
                    responses[i] = Tools.Scale(min, max, 0, 1, responses[i]);

                // Create the bar graph to show the relative responses
                CreateBarGraph(graphClassification, responses);
            }
        }

        private void btnCanvasClear_Click(object sender, EventArgs e)
        {
            canvas.Clear();
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            btnCanvasClassify_Click(this, EventArgs.Empty);
        }

        private void tbPenWidth_Scroll(object sender, EventArgs e)
        {
            canvas.PenSize = tbPenWidth.Value;
        }

        private void dgvClasses_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgvClasses.CurrentRow != null)
            {
                DiscriminantAnalysisClass dclass = (DiscriminantAnalysisClass)dgvClasses.CurrentRow.DataBoundItem;

                ImageList list = new ImageList();

                lvClass.Items.Clear();
                lvClass.LargeImageList = list;
                int[] idx = dclass.Indices;
                for (int i = 0; i < idx.Length; i++)
                {
                    Bitmap bitmap = (Bitmap)dgvAnalysisSource.Rows[idx[i]].Cells["colTrainingImage"].Value;
                    list.Images.Add(bitmap);

                    var item = new ListViewItem(String.Empty, i);
                    lvClass.Items.Add(item);
                }
            }
        }
        #endregion


        #region ZedGraph Creation
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

        public void CreateBarGraph(ZedGraphControl zgc, double[] discriminants)
        {
            GraphPane myPane = zgc.GraphPane;

            myPane.CurveList.Clear();

            myPane.Title.IsVisible = false;
            myPane.Legend.IsVisible = false;
            myPane.Border.IsVisible = false;
            myPane.Border.IsVisible = false;
            myPane.Margin.Bottom = 20f;
            myPane.Margin.Right = 20f;
            myPane.Margin.Left = 20f;
            myPane.Margin.Top = 30f;

            myPane.YAxis.Title.IsVisible = true;
            myPane.YAxis.IsVisible = true;
            myPane.YAxis.MinorGrid.IsVisible = false;
            myPane.YAxis.MajorGrid.IsVisible = false;
            myPane.YAxis.IsAxisSegmentVisible = false;
            myPane.YAxis.Scale.Max = 9.5;
            myPane.YAxis.Scale.Min = -0.5;
            myPane.YAxis.MajorGrid.IsZeroLine = false;
            myPane.YAxis.Title.Text = "Classes";
            myPane.YAxis.MinorTic.IsOpposite = false;
            myPane.YAxis.MajorTic.IsOpposite = false;
            myPane.YAxis.MinorTic.IsInside = false;
            myPane.YAxis.MajorTic.IsInside = false;
            myPane.YAxis.MinorTic.IsOutside = false;
            myPane.YAxis.MajorTic.IsOutside = false;

            myPane.XAxis.MinorTic.IsOpposite = false;
            myPane.XAxis.MajorTic.IsOpposite = false;
            myPane.XAxis.Title.IsVisible = true;
            myPane.XAxis.Title.Text = "Relative class response";
            myPane.XAxis.IsVisible = true;
            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.Max = 100;
            myPane.XAxis.IsAxisSegmentVisible = false;
            myPane.XAxis.MajorGrid.IsVisible = false;
            myPane.XAxis.MajorGrid.IsZeroLine = false;
            myPane.XAxis.MinorTic.IsOpposite = false;
            myPane.XAxis.MinorTic.IsInside = false;
            myPane.XAxis.MinorTic.IsOutside = false;
            myPane.XAxis.Scale.Format = "0'%";


            // Create data points for three BarItems using Random data
            PointPairList list = new PointPairList();

            for (int i = 0; i < discriminants.Length; i++)
                list.Add(discriminants[i] * 100, i);

            BarItem myCurve = myPane.AddBar("b", list, Color.DarkBlue);


            // Set BarBase to the YAxis for horizontal bars
            myPane.BarSettings.Base = BarBase.Y;


            zgc.AxisChange();
            zgc.Invalidate();

        }
        #endregion
       

    }
}
