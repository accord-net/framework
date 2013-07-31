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

using Accord.Imaging.Filters;
using Accord.Vision.Detection;
using Accord.Vision.Detection.Cascades;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace FaceDetection
{
    public partial class MainForm : Form
    {
        Bitmap picture = FaceDetection.Properties.Resources.judybats;

        HaarObjectDetector detector;

        public MainForm()
        {
            InitializeComponent();

            pictureBox1.Image = picture;

            cbMode.DataSource = Enum.GetValues(typeof(ObjectDetectorSearchMode));
            cbScaling.DataSource = Enum.GetValues(typeof(ObjectDetectorScalingMode));

            cbMode.SelectedItem = ObjectDetectorSearchMode.NoOverlap;
            cbScaling.SelectedItem = ObjectDetectorScalingMode.SmallerToGreater;

            toolStripStatusLabel1.Text = "Please select the detector options and click Detect to begin.";

            HaarCascade cascade = new FaceHaarCascade();
            detector = new HaarObjectDetector(cascade, 30);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            detector.SearchMode = (ObjectDetectorSearchMode)cbMode.SelectedValue;
            detector.ScalingMode = (ObjectDetectorScalingMode)cbScaling.SelectedValue;
            detector.ScalingFactor = 1.5f;
            detector.UseParallelProcessing = cbParallel.Checked;
            detector.Suppression = 2;

            Stopwatch sw = Stopwatch.StartNew();


            // Process frame to detect objects
            Rectangle[] objects = detector.ProcessFrame(picture);


            sw.Stop();


            if (objects.Length > 0)
            {
                RectanglesMarker marker = new RectanglesMarker(objects, Color.Fuchsia);
                pictureBox1.Image = marker.Apply(picture);
            }

            toolStripStatusLabel1.Text = string.Format("Completed detection of {0} objects in {1}.",
                objects.Length, sw.Elapsed);
        }



    }
}
