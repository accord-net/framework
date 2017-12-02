// Simple Shape Checker sample application
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2006-2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Reflection;

using Accord;
using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Math.Geometry;

namespace SampleApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        // Exit from application
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // On loading of the form
        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadDemo("coins.jpg");
        }

        // Open file
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ProcessImage((Bitmap)Bitmap.FromFile(openFileDialog.FileName));
                }
                catch
                {
                    MessageBox.Show("Failed loading selected image file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Load first demo image
        private void loadDemoImage1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDemo("coins.jpg");
        }

        // Load second demo image
        private void loadDemoImage2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDemo("demo.png");
        }

        // Load third demo image
        private void loadDemoImage3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDemo("demo1.png");
        }

        // Load fourth demo image
        private void loadDemoImage4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDemo("demo2.png");
        }

        // Load one of the embedded demo image
        private void LoadDemo(string embeddedFileName)
        {
            // load arrow bitmap
            Assembly assembly = this.GetType().Assembly;
            Bitmap image = new Bitmap(assembly.GetManifestResourceStream("SampleApp." + embeddedFileName));
            ProcessImage(image);
        }

        // Process image
        private void ProcessImage(Bitmap bitmap)
        {
            // lock image
            BitmapData bitmapData = bitmap.LockBits(ImageLockMode.ReadWrite);

            // step 1 - turn background to black
            ColorFiltering colorFilter = new ColorFiltering();

            colorFilter.Red = new IntRange(0, 64);
            colorFilter.Green = new IntRange(0, 64);
            colorFilter.Blue = new IntRange(0, 64);
            colorFilter.FillOutsideRange = false;

            colorFilter.ApplyInPlace(bitmapData);

            // step 2 - locating objects
            BlobCounter blobCounter = new BlobCounter();

            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 5;
            blobCounter.MinWidth = 5;

            blobCounter.ProcessImage(bitmapData);
            Blob[] blobs = blobCounter.GetObjectsInformation();
            bitmap.UnlockBits(bitmapData);

            // step 3 - check objects' type and highlight
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

            Graphics g = Graphics.FromImage(bitmap);
            Pen yellowPen = new Pen(Color.Yellow, 2); // circles
            Pen redPen = new Pen(Color.Red, 2);       // quadrilateral
            Pen brownPen = new Pen(Color.Brown, 2);   // quadrilateral with known sub-type
            Pen greenPen = new Pen(Color.Green, 2);   // known triangle
            Pen bluePen = new Pen(Color.Blue, 2);     // triangle

            for (int i = 0; i < blobs.Length; i++)
            {
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);

                Accord.Point center;
                float radius;

                // is circle ?
                if (shapeChecker.IsCircle(edgePoints, out center, out radius))
                {
                    g.DrawEllipse(yellowPen,
                        (float)(center.X - radius), (float)(center.Y - radius),
                        (float)(radius * 2), (float)(radius * 2));
                }
                else
                {
                    List<IntPoint> corners;

                    // is triangle or quadrilateral
                    if (shapeChecker.IsConvexPolygon(edgePoints, out corners))
                    {
                        // get sub-type
                        PolygonSubType subType = shapeChecker.CheckPolygonSubType(corners);

                        Pen pen;

                        if (subType == PolygonSubType.Unknown)
                        {
                            pen = (corners.Count == 4) ? redPen : bluePen;
                        }
                        else
                        {
                            pen = (corners.Count == 4) ? brownPen : greenPen;
                        }

                        g.DrawPolygon(pen, ToPointsArray(corners));
                    }
                }
            }

            yellowPen.Dispose();
            redPen.Dispose();
            greenPen.Dispose();
            bluePen.Dispose();
            brownPen.Dispose();
            g.Dispose();

            // put new image to clipboard
            Clipboard.SetDataObject(bitmap);
            // and to picture box
            pictureBox.Image = bitmap;

            UpdatePictureBoxPosition();
        }

        // Size of main panel has changed
        private void splitContainer_Panel2_Resize(object sender, EventArgs e)
        {
            UpdatePictureBoxPosition();
        }

        // Update size and position of picture box control
        private void UpdatePictureBoxPosition()
        {
            int imageWidth;
            int imageHeight;

            if (pictureBox.Image == null)
            {
                imageWidth = 320;
                imageHeight = 240;
            }
            else
            {
                imageWidth = pictureBox.Image.Width;
                imageHeight = pictureBox.Image.Height;
            }

            Rectangle rc = splitContainer.Panel2.ClientRectangle;

            pictureBox.SuspendLayout();
            pictureBox.Location = new System.Drawing.Point((rc.Width - imageWidth - 2) / 2, (rc.Height - imageHeight - 2) / 2);
            pictureBox.Size = new Size(imageWidth + 2, imageHeight + 2);
            pictureBox.ResumeLayout();
        }

        // Conver list of AForge.NET's points to array of .NET points
        private System.Drawing.Point[] ToPointsArray(List<IntPoint> points)
        {
            System.Drawing.Point[] array = new System.Drawing.Point[points.Count];

            for (int i = 0; i < points.Count; i++)
                array[i] = new System.Drawing.Point(points[i].X, points[i].Y);

            return array;
        }

        // Show about form
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm form = new AboutForm();

            form.ShowDialog();
        }
    }
}
