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
using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Math;
using AForge;
using AForge.Imaging.Filters;
using Accord.MachineLearning;
using System.Linq;
using System.Collections.Generic;

namespace Panorama
{
    public partial class MainForm : Form
    {
        private Bitmap img1 = Panorama.Properties.Resources.UFSCar_Lake1;
        private Bitmap img2 = Panorama.Properties.Resources.UFSCar_Lake2;

        private FastRetinaKeypoint[] keyPoints1;
        private FastRetinaKeypoint[] keyPoints2;

        private IntPoint[] correlationPoints1;
        private IntPoint[] correlationPoints2;

        private MatrixH homography;


        public MainForm()
        {
            InitializeComponent();

            // Concatenate and show entire image at start
            Concatenate concatenate = new Concatenate(img1);
            pictureBox.Image = concatenate.Apply(img2);
        }


        private void btnFreak_Click(object sender, EventArgs e)
        {
            // Step 1: Detect feature points using FREAK Features Detector
            FastRetinaKeypointDetector freak = new FastRetinaKeypointDetector();

            keyPoints1 = freak.ProcessImage(img1).ToArray();
            keyPoints2 = freak.ProcessImage(img2).ToArray();

            // Show the marked points in the original images
            Bitmap img1mark = new PointsMarker(keyPoints1.Apply(p => (IntPoint)p)).Apply(img1);
            Bitmap img2mark = new PointsMarker(keyPoints2.Apply(p => (IntPoint)p)).Apply(img2);

            // Concatenate the two images together in a single image (just to show on screen)
            Concatenate concatenate = new Concatenate(img1mark);
            pictureBox.Image = concatenate.Apply(img2mark);
        }

        private void btnCorrelation_Click(object sender, EventArgs e)
        {
            if (keyPoints1 == null)
            {
                MessageBox.Show("Please, click FREAK button first! :-)");
                return;
            }

            // Step 2: Match feature points using a k-NN
            var matcher = new KNearestNeighborMatching<byte[]>(5, Distance.BitwiseHamming);
            IntPoint[][] matches = matcher.Match(keyPoints1, keyPoints2);

            // Get the two sets of points
            correlationPoints1 = matches[0];
            correlationPoints2 = matches[1];

            // Concatenate the two images in a single image (just to show on screen)
            Concatenate concat = new Concatenate(img1);
            Bitmap img3 = concat.Apply(img2);

            // Show the marked correlations in the concatenated image
            PairsMarker pairs = new PairsMarker(
                correlationPoints1, // Add image1's width to the X points to show the markings correctly
                correlationPoints2.Apply(p => new IntPoint(p.X + img1.Width, p.Y)));

            pictureBox.Image = pairs.Apply(img3);
        }

        private void btnRansac_Click(object sender, EventArgs e)
        {
            if (correlationPoints1 == null)
            {
                MessageBox.Show("Please, click Nearest Neighbor button first! :-)");
                return;
            }

            if (correlationPoints1.Length < 4 || correlationPoints2.Length < 4)
            {
                MessageBox.Show("Insufficient points to attempt a fit.");
                return;
            }

            // Step 3: Create the homography matrix using a robust estimator
            RansacHomographyEstimator ransac = new RansacHomographyEstimator(0.001, 0.99);
            homography = ransac.Estimate(correlationPoints1, correlationPoints2);

            // Plot RANSAC results against correlation results
            IntPoint[] inliers1 = correlationPoints1.Submatrix(ransac.Inliers);
            IntPoint[] inliers2 = correlationPoints2.Submatrix(ransac.Inliers);

            // Concatenate the two images in a single image (just to show on screen)
            Concatenate concat = new Concatenate(img1);
            Bitmap img3 = concat.Apply(img2);

            // Show the marked correlations in the concatenated image
            PairsMarker pairs = new PairsMarker(
                inliers1, // Add image1's width to the X points to show the markings correctly
                inliers2.Apply(p => new IntPoint(p.X + img1.Width, p.Y)));

            pictureBox.Image = pairs.Apply(img3);
        }

        private void btnBlend_Click(object sender, EventArgs e)
        {
            if (homography == null)
            {
                MessageBox.Show("Please, click RANSAC button first! :-)");
                return;
            }

            // Step 4: Project and blend the second image using the homography
            Blend blend = new Blend(homography, img1);
            pictureBox.Image = blend.Apply(img2);
        }

        private void btnDoItAll_Click(object sender, EventArgs e)
        {
            // Do it all
            btnFreak_Click(sender, e);
            btnCorrelation_Click(sender, e);
            btnRansac_Click(sender, e);
            btnBlend_Click(sender, e);
        }


    }
}
