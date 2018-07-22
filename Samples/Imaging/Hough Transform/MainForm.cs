// Hough line and circle transformation demo
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2006-2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

using AForge;
using Accord.Imaging;
using Accord.Imaging.Filters;
using System.IO;
using Accord;

namespace SampleApp
{
    public partial class MainForm : Form
    {
        // binarization filtering sequence
        private FiltersSequence filter = new FiltersSequence(
            Grayscale.CommonAlgorithms.BT709,
            new NiblackThreshold(),
            new Invert()
        );

        HoughLineTransformation lineTransform = new HoughLineTransformation();
        HoughCircleTransformation circleTransform = new HoughCircleTransformation(35);


        public MainForm()
        {
            InitializeComponent();

            lineTransform.MinLineIntensity = 10;
            circleTransform.MinCircleIntensity = 20;

            openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Resources");
        }

        // Exit from application
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Open image file
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // show file open dialog
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // load image
                    Bitmap tempImage = (Bitmap)Bitmap.FromFile(openFileDialog.FileName);
                    Bitmap image = Accord.Imaging.Image.Clone(tempImage, PixelFormat.Format24bppRgb);
                    tempImage.Dispose();

                    // lock the source image
                    BitmapData sourceData = image.LockBits(ImageLockMode.ReadOnly);

                    // binarize the image
                    UnmanagedImage binarySource = filter.Apply(new UnmanagedImage(sourceData));

                    // apply Hough line transform
                    lineTransform.ProcessImage(binarySource);

                    // get lines using relative intensity
                    HoughLine[] lines = lineTransform.GetLinesByRelativeIntensity(0.9);

                    foreach (HoughLine line in lines)
                    {
                        string s = string.Format("Theta = {0}, R = {1}, I = {2} ({3})", line.Theta, line.Radius, line.Intensity, line.RelativeIntensity);
                        System.Diagnostics.Debug.WriteLine(s);

                        // uncomment to highlight detected lines
                        // get line's radius and theta values
                        int r = line.Radius;
                        double t = line.Theta;

                        // check if line is in lower part of the image
                        if (r < 0)
                        {
                            t += 180;
                            r = -r;
                        }

                        // convert degrees to radians
                        t = (t / 180) * Math.PI;

                        // get image centers (all coordinate are measured relative
                        // to center)
                        int w2 = image.Width / 2;
                        int h2 = image.Height / 2;

                        double x0 = 0, x1 = 0, y0 = 0, y1 = 0;

                        if (line.Theta != 0)
                        {
                            // none vertical line
                            x0 = -w2; // most left point
                            x1 = w2;  // most right point

                            // calculate corresponding y values
                            y0 = (-Math.Cos(t) * x0 + r) / Math.Sin(t);
                            y1 = (-Math.Cos(t) * x1 + r) / Math.Sin(t);
                        }
                        else
                        {
                            // vertical line
                            x0 = line.Radius;
                            x1 = line.Radius;

                            y0 = h2;
                            y1 = -h2;
                        }

                        // draw line on the image
                        Drawing.Line(sourceData,
                            new IntPoint((int)x0 + w2, h2 - (int)y0),
                            new IntPoint((int)x1 + w2, h2 - (int)y1),
                            Color.Red);
                    }

                    System.Diagnostics.Debug.WriteLine("Found lines: " + lineTransform.LinesCount);
                    System.Diagnostics.Debug.WriteLine("Max intensity: " + lineTransform.MaxIntensity);

                    // apply Hough circle transform
                    circleTransform.ProcessImage(binarySource);
                    // get circles using relative intensity
                    HoughCircle[] circles = circleTransform.GetCirclesByRelativeIntensity(0.9);

                    foreach (HoughCircle circle in circles)
                    {
                        string s = string.Format("X = {0}, Y = {1}, I = {2} ({3})", circle.X, circle.Y, circle.Intensity, circle.RelativeIntensity);
                        System.Diagnostics.Debug.WriteLine(s);
                    }

                    System.Diagnostics.Debug.WriteLine("Found circles: " + circleTransform.CirclesCount);
                    System.Diagnostics.Debug.WriteLine("Max intensity: " + circleTransform.MaxIntensity);

                    // unlock source image
                    image.UnlockBits(sourceData);
                    // dispose temporary binary source image
                    binarySource.Dispose();

                    // show images
                    sourcePictureBox.Image = image;
                    houghLinePictureBox.Image = lineTransform.ToBitmap();
                    houghCirclePictureBox.Image = circleTransform.ToBitmap();
                }
            }
            catch
            {
                MessageBox.Show("Failed loading the image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
