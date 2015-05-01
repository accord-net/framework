// Motion Detection sample application
// AForge.NET Framework
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
        public Rectangle[] MotionRectangles
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

        // On rectangle button click
        private void rectangleButton_Click( object sender, EventArgs e )
        {
            DrawingMode currentMode = defineRegionsControl.DrawingMode;

            // change current mode
            currentMode = ( currentMode == DrawingMode.Rectangular ) ? DrawingMode.None : DrawingMode.Rectangular;
            // update current mode
            defineRegionsControl.DrawingMode = currentMode;
            // change button status
            rectangleButton.Checked = ( currentMode == DrawingMode.Rectangular );
        }

        // New rectangle definition was finished
        private void defineRegionsControl_NewRectangleHandler( object sender, Rectangle rect )
        {
            rectangleButton.Checked = false;
        }

        // On clear button click
        private void clearButton_Click( object sender, EventArgs e )
        {
            defineRegionsControl.RemoveAllRegions( );
        }
    }
}