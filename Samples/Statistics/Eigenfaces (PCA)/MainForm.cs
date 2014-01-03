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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Accord.Controls;
using Accord.Imaging.Converters;
using Accord.MachineLearning;
using Accord.Math;
using Accord.Statistics.Analysis;
using ZedGraph;

namespace Eigenfaces.PCA
{
    public partial class MainForm : Form
    {

        PrincipalComponentAnalysis pca;

        MinimumMeanDistanceClassifier classifier;

        public MainForm()
        {
            InitializeComponent();

            dgvPrincipalComponents.AutoGenerateColumns = false;
        }



        void btnCompute_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.Clear();


            // Extract feature vectors
            double[][] hands = extract();

            // Create a new Principal Component Analysis object
            pca = new PrincipalComponentAnalysis(hands, AnalysisMethod.Center);

            // Compute it
            pca.Compute();

            // Now we will plot the Eigenvectors as images
            ArrayToImage reverse = new ArrayToImage(32, 32);


            // For each Principal Component
            for (int i = 0; i < pca.Components.Count; i++)
            {
                // We will extract its Eigenvector
                double[] vector = pca.Components[i].Eigenvector;

                // Normalize its values
                reverse.Max = vector.Max();
                reverse.Min = vector.Min();

                // Then arrange each vector value as if it was a pixel
                Bitmap eigenHand; reverse.Convert(vector, out eigenHand);

                // This will give the Eigenhands
                dataGridView2.Rows.Add(eigenHand, pca.Components[i].Proportion);
            }

            // Populates components overview with analysis data
            dgvPrincipalComponents.DataSource = pca.Components;

            CreateComponentCumulativeDistributionGraph(graphCurve);
            CreateComponentDistributionGraph(graphShare);

            btnCreateProjection.Enabled = true;
        }


        private void btnFeature_Click(object sender, EventArgs e)
        {
            if (pca == null)
            {
                MessageBox.Show("Please compute the analysis first!");
                return;
            }

            ImageToArray converter = new ImageToArray(min: -1, max: +1);

            int rows = dataGridView3.Rows.Count;
            double[][] inputs = new double[rows][];
            double[][] features = new double[rows][];
            int[] outputs = new int[rows];

            int index = 0;
            foreach (DataGridViewRow row in dataGridView3.Rows)
            {
                Bitmap image = row.Cells["colHand2"].Value as Bitmap;
                int label = (int)row.Cells["colLabel2"].Value;

                double[] input;
                converter.Convert(image, out input);

                double[] feature = pca.Transform(input);

                row.Cells["colProjection"].Value = feature.ToString("N2");

                row.Tag = feature;
                inputs[index] = input;
                features[index] = feature;
                outputs[index] = label;
                index++;
            }

            classifier = new MinimumMeanDistanceClassifier(features, outputs);

            btnClassify.Enabled = true;
        }

        private void btnClassify_Click(object sender, EventArgs e)
        {
            if (classifier == null)
            {
                MessageBox.Show("Please create a classifier first!");
                return;
            }

            foreach (DataGridViewRow row in dataGridView3.Rows)
            {
                double[] feature = (double[])row.Tag;

                int output = classifier.Compute(feature);

                row.Cells["colClassification"].Value = ((char)(output + 'A')).ToString();

                int expected = (int)row.Cells["colLabel2"].Value;
                row.DefaultCellStyle.BackColor = (expected == output) ? Color.LightGreen : Color.White;
            }
        }



        double[][] extract()
        {
            double[][] hands = new double[dataGridView1.Rows.Count][];
            ImageToArray converter = new ImageToArray(min: -1, max: +1);

            int index = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                Bitmap image = row.Cells["colHand"].Value as Bitmap;
                converter.Convert(image, out hands[index]);
                index++;
            }

            return hands;
        }




        private void MainForm_Load(object sender, EventArgs e)
        {
            // Open Resources folder
            var path = new DirectoryInfo(Path.Combine(Application.StartupPath, "Hands"));


            int currentClassLabel = 0;

            // For every class folder
            foreach (var classFolder in path.EnumerateDirectories())
            {
                string name = classFolder.Name;

                // For each file in the class folder
                FileInfo[] files = GetFilesByExtensions(classFolder, ".bmp").ToArray();

                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo file = files[i];

                    Bitmap image = (Bitmap)Bitmap.FromFile(file.FullName);

                    dataGridView1.Rows.Add(name, currentClassLabel, image);
                    dataGridView3.Rows.Add(name, currentClassLabel, image);
                }

                currentClassLabel++;
            }
        }

        public static IEnumerable<FileInfo> GetFilesByExtensions(DirectoryInfo dir, params string[] extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException("extensions");
            IEnumerable<FileInfo> files = dir.EnumerateFiles();
            return files.Where(f => extensions.Contains(f.Extension));
        }



        #region Graphs
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
            for (int i = 0; i < pca.Components.Count; i++)
            {
                list.Add(pca.Components[i].Index, pca.Components[i].CumulativeProportion);
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
            ColorSequenceCollection sequence = new ColorSequenceCollection();
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
            for (int i = 0; i < pca.Components.Count; i++)
            {
                myPane.AddPieSlice(pca.Components[i].Proportion,
                    sequence[i], 0.1, pca.Components[i].Index.ToString());
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
        #endregion

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }
    }
}
