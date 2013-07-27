// Face Tracking sample application
// Accord.NET Framework
//
// Based on code from the:
//   Motion Detection sample application
//   AForge.NET Framework
//   http://www.aforgenet.com/framework/
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MotionDetectorSample
{
    public partial class MotionRegionsForm : Form
    {
        // Video frame sample to show
        public Bitmap VideoFrame
        {
            set { defineRegionsControl.BackgroundImage = value; }
        }

        // Motion rectangles
        public Rectangle[] ObjectRectangles
        {
            get { return defineRegionsControl.Rectangles; }
            set { defineRegionsControl.Rectangles = value; }
        }

        // Class constructor
        public MotionRegionsForm( )
        {
            InitializeComponent( );

            defineRegionsControl.OnNewRectangle += new NewRectangleHandler( defineRegionsControl_NewRectangleHandler );
        }

        // On first displaying of the form
        protected override void OnLoad( EventArgs e )
        {
            // get video frame dimension
            if ( defineRegionsControl.BackgroundImage != null )
            {
                int imageWidth  = defineRegionsControl.BackgroundImage.Width;
                int imageHeight = defineRegionsControl.BackgroundImage.Height;

                // resize region definition control
                defineRegionsControl.Size = new Size( imageWidth + 2, imageHeight + 2 );
                // resize window
                this.Size = new Size( imageWidth + 2 + 26, imageHeight + 2 + 118 );
            }

            base.OnLoad( e );
        }

        // New rectangle definition was finished
        private void defineRegionsControl_NewRectangleHandler( object sender, Rectangle rect )
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

    }
}