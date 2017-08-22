// Accord.NET Sample Applications
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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Accord.Imaging.Filters;
using Accord.Vision.Detection;
using Accord.Vision.Detection.Cascades;
using SampleApp.Properties;
using System.Net;
using System.IO;

namespace SampleApp
{
    public partial class MainForm : Form
    {
        Bitmap picture = Resources.judybats;

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

        private void loadImageButton_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] data = null;
                using (WebClient client = new WebClient())
                {
                    string url = urlTextBox.Text;
                    data = client.DownloadData(new Uri(url));
                }

                using (MemoryStream mem = new MemoryStream(data))
                {
                    using (var image = Image.FromStream(mem))
                    {
                        pictureBox1.Image = this.picture = new Bitmap(image);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                toolStripStatusLabel1.Text = "LoadImageError: " + ex.Message;
                pictureBox1.Image = this.picture = new Bitmap(1, 1);
            }
        }
    }
}
