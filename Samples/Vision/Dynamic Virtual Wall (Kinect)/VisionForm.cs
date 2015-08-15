using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Accord.Controls.Vision;
using AForge.Imaging;
using Accord.Vision.Tracking;
using System.Drawing.Imaging;
using Accord.Imaging.Filters;

namespace KinectController
{
    public partial class VisionForm : Form
    {
        MainForm parent;
        HeadController controller;
        RectanglesMarker marker = new RectanglesMarker(Color.Blue);

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
        }

        void controller_HeadMove(object sender, HeadEventArgs e)
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
}
