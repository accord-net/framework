﻿// Accord.NET Sample Applications
// http://accord-framework.net
//
// Copyright © 2009-2017, César Souza
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
using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Math;
using AForge;
using Accord.Math.Distances;
using Accord;
using System.Collections.Generic;
using System.Linq;

namespace SampleApp
{
    public partial class MainForm : Form
    {
        private Bitmap img1 = SampleApp.Properties.Resources.UFSCar_Lake1;
        private Bitmap img2 = SampleApp.Properties.Resources.UFSCar_Lake2;

        private IEnumerable<FastRetinaKeypoint> keyPoints1;
        private IEnumerable<FastRetinaKeypoint> keyPoints2;

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

            keyPoints1 = freak.Transform(img1);
            keyPoints2 = freak.Transform(img2);

            // Show the marked points in the original images
            // TODO: The following construct can be simplified
            Bitmap img1mark = new PointsMarker(keyPoints1.Select(x => (IFeaturePoint)x).ToList()).Apply(img1);
            Bitmap img2mark = new PointsMarker(keyPoints2.Select(x => (IFeaturePoint)x).ToList()).Apply(img2);

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
            var matcher = new KNearestNeighborMatching<byte[]>(5, new Hamming());
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
            IntPoint[] inliers1 = correlationPoints1.Get(ransac.Inliers);
            IntPoint[] inliers2 = correlationPoints2.Get(ransac.Inliers);

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
