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
using System.Drawing;
using System.Windows.Forms;
using Accord.Imaging.Converters;
using Accord.MachineLearning;
using Accord.Math;
using Accord.Statistics.Distributions.DensityKernels;

namespace Clustering.K_Means
{
    /// <summary>
    ///   K-Means / Mean-Shift color clusterization sample application. This
    ///   application uses either K-Means or Mean-Shift to reduce the number
    ///   of colors in a sample image.
    /// </summary>
    /// 
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///   Determines which radio button is selected 
        ///   and calls the appropriate algorithm.
        /// </summary>
        /// 
        private void btnRun_Click(object sender, EventArgs e)
        {
            if (radioClusters.Checked)
                runKMeans();
            else runMeanShift();
        }

        /// <summary>
        ///   Runs the K-Means algorithm.
        /// </summary>
        /// 
        private void runKMeans()
        {
            // Retrieve the number of clusters
            int k = (int)numClusters.Value;

            // Load original image
            Bitmap image = Properties.Resources.leaf;

            // Create converters
            ImageToArray imageToArray = new ImageToArray(min: -1, max: +1);
            ArrayToImage arrayToImage = new ArrayToImage(image.Width, image.Height, min: -1, max: +1);

            // Transform the image into an array of pixel values
            double[][] pixels; imageToArray.Convert(image, out pixels);


            // Create a K-Means algorithm using given k and a
            //  square Euclidean distance as distance metric.
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

        /// <summary>
        ///   Runs the Mean-Shift algorithm.
        /// </summary>
        /// 
        private void runMeanShift()
        {
            int pixelSize = 3;

            // Retrieve the kernel bandwidth
            double sigma = (double)numBandwidth.Value;

            // Load original image
            Bitmap image = Properties.Resources.leaf;

            // Create converters
            ImageToArray imageToArray = new ImageToArray(min: -1, max: +1);
            ArrayToImage arrayToImage = new ArrayToImage(image.Width, image.Height, min: -1, max: +1);

            // Transform the image into an array of pixel values
            double[][] pixels; imageToArray.Convert(image, out pixels);


            // Create a MeanShift algorithm using given bandwidth
            //   and a Gaussian density kernel as kernel function.
            MeanShift meanShift = new MeanShift(pixelSize, 
                new GaussianKernel(pixelSize), sigma);
            
            // Compute the mean-shift algorithm until the difference in
            //  shifting means between two iterations is below 0.05
            int[] idx = meanShift.Compute(pixels, 0.05, maxIterations: 10);

            
            // Replace every pixel with its corresponding centroid
            pixels.ApplyInPlace((x, i) => meanShift.Clusters.Modes[idx[i]]);

            // Show resulting image in the picture box
            Bitmap result; arrayToImage.Convert(pixels, out result);

            pictureBox.Image = result;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            pictureBox.Image = Properties.Resources.leaf;
        }
    }
}
