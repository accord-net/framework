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
using System.Drawing;
using System.Windows.Forms;
using Accord.MachineLearning;
using Accord.Math;
using Accord.Statistics.Distributions.Multivariate;
using ZedGraph;
using Accord.Controls;

namespace GMM
{
    public partial class MainForm : Form
    {
        // Colors used in the pie graphics
        ColorSequenceCollection colors = new ColorSequenceCollection();

        int k;
        double[][] mixture;

        KMeans kmeans;

        public MainForm()
        {
            InitializeComponent();
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            btnGenerateRandom_Click(this, e);
        }

        private void btnGenerateRandom_Click(object sender, EventArgs e)
        {
            k = (int)numClusters.Value;

            // Generate data with n Gaussian distributions
            double[][][] data = new double[k][][];

            for (int i = 0; i < k; i++)
            {
                // Create random centroid to place the Gaussian distribution
                double[] mean = Matrix.Random(2, -6.0, +6.0);

                // Create random covariance matrix for the distribution
                double[,] covariance = Accord.Statistics.Tools.RandomCovariance(2, -5, 5);

                // Create the Gaussian distribution
                var gaussian = new MultivariateNormalDistribution(mean, covariance);

                int samples = Accord.Math.Tools.Random.Next(150, 250);
                data[i] = gaussian.Generate(samples);
            }

            // Join the generated data
            mixture = Matrix.Stack(data);

            // Update the scatterplot
            CreateScatterplot(graph, mixture, k);

            // Forget previous initialization
            kmeans = null;
        }

        private void btnCompute_Click(object sender, EventArgs e)
        {
            // Create a new Gaussian Mixture Model
            GaussianMixtureModel gmm = new GaussianMixtureModel(k);

            // If available, initialize with k-means
            if (kmeans != null) gmm.Initialize(kmeans);

            // Compute the model
            gmm.Compute(mixture);

            // Classify all instances in mixture data
            int[] classifications = gmm.Gaussians.Nearest(mixture);

            // Draw the classifications
            updateGraph(classifications);
        }

        private void updateGraph(int[] classifications)
        {
            // Paint the clusters accordingly
            for (int i = 0; i < k + 1; i++)
                graph.GraphPane.CurveList[i].Clear();

            for (int j = 0; j < mixture.Length; j++)
            {
                int c = classifications[j];

                var curveList = graph.GraphPane.CurveList[c + 1];
                double[] point = mixture[j];
                curveList.AddPoint(point[0], point[1]);
            }

            graph.Invalidate();
        }

        private void btnInitialize_Click(object sender, EventArgs e)
        {
            kmeans = new KMeans(k);

            kmeans.Compute(mixture);

            // Classify all instances in mixture data
            int[] classifications = kmeans.Clusters.Nearest(mixture);

            // Draw the classifications
            updateGraph(classifications);
        }


        public void CreateScatterplot(ZedGraphControl zgc, double[][] graph, int n)
        {
            GraphPane myPane = zgc.GraphPane;
            myPane.CurveList.Clear();

            // Set graph pane object
            myPane.Title.Text = "Normal (Gaussian) Distributions";
            myPane.XAxis.Title.Text = "X";
            myPane.YAxis.Title.Text = "Y";
            myPane.XAxis.Scale.Max = 10;
            myPane.XAxis.Scale.Min = -10;
            myPane.YAxis.Scale.Max = 10;
            myPane.YAxis.Scale.Min = -10;
            myPane.XAxis.IsAxisSegmentVisible = false;
            myPane.YAxis.IsAxisSegmentVisible = false;
            myPane.YAxis.IsVisible = false;
            myPane.XAxis.IsVisible = false;
            myPane.Border.IsVisible = false;


            // Create mixture pairs
            PointPairList list = new PointPairList();
            for (int i = 0; i < graph.Length; i++)
                list.Add(graph[i][0], graph[i][1]);


            // Add the curve for the mixture points
            LineItem myCurve = myPane.AddCurve("Mixture", list, Color.Gray, SymbolType.Diamond);
            myCurve.Line.IsVisible = false;
            myCurve.Symbol.Border.IsVisible = false;
            myCurve.Symbol.Fill = new Fill(Color.Gray);

            for (int i = 0; i < n; i++)
            {
                // Add curves for the clusters to be detected
                Color color = colors[i];
                myCurve = myPane.AddCurve("D" + (i + 1), new PointPairList(), color, SymbolType.Diamond);
                myCurve.Line.IsVisible = false;
                myCurve.Symbol.Border.IsVisible = false;
                myCurve.Symbol.Fill = new Fill(color);
            }

            // Fill the background of the chart rect and pane
            myPane.Fill = new Fill(Color.WhiteSmoke);

            zgc.AxisChange();
            zgc.Invalidate();
        }



    }
}
