// Accord.NET Sample Applications
// http://accord-framework.net
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
using System.Drawing.Imaging;
using System.Windows.Forms;
using Accord.Controls.Vision;
using Accord.Imaging.Filters;
using Accord.Vision.Tracking;
using AForge.Imaging.Filters;
using AForge.Video;

namespace Controller
{
    public partial class VisionForm : Form
    {
        MainForm parent;
        HeadController controller;
        RectanglesMarker marker = new RectanglesMarker(Color.Blue);

        bool backproj = true;

        public VisionForm()
        {
            InitializeComponent();
        }

        public VisionForm(MainForm mainForm)
            : this()
        {
            this.parent = mainForm;

            this.controller = mainForm.controller;
            this.controller.HeadMove += controller_HeadMove;
            this.controller.NewFrame += controller_NewFrame;
        }

        void controller_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (!backproj)
            {
                Bitmap image = eventArgs.Frame;

                if (image == null)
                    return;

                if (parent.faceForm != null && !parent.faceForm.IsDisposed)
                {
                    MatchingTracker matching = parent.faceForm.faceController.Tracker as MatchingTracker;

                    Rectangle rect = new Rectangle(
                        matching.TrackingObject.Center.X,
                        0,
                        image.Width - matching.TrackingObject.Center.X,
                        matching.TrackingObject.Center.Y);

                 
                    rect.Intersect(new Rectangle(0, 0, image.Width, image.Height));

                     marker.Rectangles = new[] { matching.TrackingObject.Rectangle };
                     image = marker.Apply(image);
                }


                pictureBox.Image = image;
            }
        }


        void controller_HeadMove(object sender, HeadEventArgs e)
        {
            if (controller == null || controller.Tracker == null)
                return;

            if (backproj)
            {
                try
                {
                    Camshift camshift = controller.Tracker as Camshift;

                    Bitmap backprojection = camshift.GetBackprojection(
                        PixelFormat.Format24bppRgb, camshift.TrackingObject.Rectangle);

                    if (parent.faceForm != null && !parent.faceForm.IsDisposed)
                    {
                        MatchingTracker matching = parent.faceForm.faceController.Tracker as MatchingTracker;

                        marker.Rectangles = new[] { matching.TrackingObject.Rectangle };
                        marker.ApplyInPlace(backprojection);
                    }


                    pictureBox.Image = backprojection;
                }
                catch
                {
                    pictureBox.Image = null;
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (backproj)
            {
                button1.Text = "Standard";
                backproj = false;
            }
            else
            {
                button1.Text = "Backprojection";
                backproj = true;
            }
        }
    }
}
