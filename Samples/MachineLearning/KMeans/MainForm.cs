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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Accord.MachineLearning;
using Accord.Imaging;
using Accord.Math;
using Accord.Statistics;
using Accord.Imaging.Converters;
using Accord.Statistics.Distributions.DensityKernels;

namespace Clustering
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (radioClusters.Checked)
                kmeans();

            else
                meanshift();
        }

        private void kmeans()
        {
            // Retrieve the number of clusters
            int k = (int)numClusters.Value;

            // Load original image
            Bitmap image = Properties.Resources.leaf;

            // Create conversors
            ImageToArray imageToArray = new ImageToArray(min: -1, max: +1);
            ArrayToImage arrayToImage = new ArrayToImage(image.Width, image.Height, min: -1, max: +1);

            // Transform the image into an array of pixel values
            double[][] pixels; imageToArray.Convert(image, out pixels);


            // Create a K-Means algorithm using given k and a
            //  square euclidean distance as distance metric.
            KMeans kmeans = new KMeans(k, Distance.SquareEuclidean);

            // Compute the K-Means algorithm until the difference in
            //  cluster centroids between two iterations is below 0.05
            int[] idx = kmeans.Compute(pixels, 0.05);


            // Replace every pixel with its corresponding centroid
            pixels.ApplyInPlace((x, i) => kmeans.Clusters.Centroids[idx[i]]);

            // Show resulting image in the picture box
            Bitmap result; arrayToImage.Convert(pixels, out result);

            pictureBox.Image = result;
        }

        private void meanshift()
        {
            int pixelSize = 3;

            // Retrieve the kernel bandwidth
            double sigma = (double)numBandwidth.Value;

            // Load original image
            Bitmap image = Properties.Resources.leaf;

            // Create conversors
            ImageToArray imageToArray = new ImageToArray(min: -1, max: +1);
            ArrayToImage arrayToImage = new ArrayToImage(image.Width, image.Height, min: -1, max: +1);

            // Transform the image into an array of pixel values
            double[][] pixels; imageToArray.Convert(image, out pixels);


            // Create a MeanShift algorithm using given bandwidth
            //   and a Gaussian density kernel as kernel function.
            MeanShift meanShift = new MeanShift(pixelSize, new GaussianKernel(3), sigma);


            // Compute the mean-shift algorithm until the difference in
            //  shifting means between two iterations is below 0.05
            int[] idx = meanShift.Compute(pixels, 0.05, maxIterations: 10);


            // Replace every pixel with its corresponding centroid
            pixels.ApplyInPlace((x, i) => meanShift.Clusters.Modes[idx[i]]);

            // Show resulting image in the picture box
            Bitmap result; arrayToImage.Convert(pixels, out result);

            pictureBox.Image = result;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox.Image = Properties.Resources.leaf;
        }
    }
}
