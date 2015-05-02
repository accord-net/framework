// Surveyor SVS test application
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2007-2010
// andrew.kirillov@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using AForge.Imaging.Filters;

namespace SVSTest
{
    public partial class StereoViewForm : Form
    {
        private ManualResetEvent leftFrameIsAvailable;
        private ManualResetEvent rightFrameIsAvailable;

        private Thread backgroundThread;

        private Bitmap leftFrame;
        private Bitmap rightFrame;

        private StereoAnaglyph stereoFilter = new StereoAnaglyph( );

        private bool needToExit = false;

        public StereoViewForm( )
        {
            InitializeComponent( );

            leftFrameIsAvailable  = new ManualResetEvent( false );
            rightFrameIsAvailable = new ManualResetEvent( false );

            backgroundThread = new Thread( new ThreadStart( stereoThread ) );
            backgroundThread.Start( );
        }

        // Closing the form
        private void StereoViewForm_FormClosing( object sender, FormClosingEventArgs e )
        {
            needToExit = true;

            leftFrameIsAvailable.Set( );
            rightFrameIsAvailable.Set( );

            backgroundThread.Join( );
        }

        // New left frame has arrived
        public void OnNewLeftFrame( object sender, ref Bitmap image )
        {
            lock ( this )
            {
                if ( leftFrame != null )
                    leftFrame.Dispose( );

                leftFrame = AForge.Imaging.Image.Clone( image );

                leftFrameIsAvailable.Set( );
            }
        }

        // New right frame has arrived
        public void OnNewRightFrame( object sender, ref Bitmap image )
        {
            lock ( this )
            {
                if ( rightFrame != null )
                    rightFrame.Dispose( );

                rightFrame = AForge.Imaging.Image.Clone( image );

                rightFrameIsAvailable.Set( );
            }
        }

        private void stereoThread( )
        {
            while ( true )
            {
                leftFrameIsAvailable.WaitOne( );
                rightFrameIsAvailable.WaitOne( );

                if ( needToExit )
                    break;

                if ( ( leftFrame.Width  != pictureBox.Width - 2 ) ||
                     ( leftFrame.Height != pictureBox.Height - 2 ) )
                {
                    UpdateWindowSize( );
                }

                lock ( this )
                {
                    try
                    {
                        Image old = pictureBox.Image;

                        // build stereo anaglyph
                        stereoFilter.OverlayImage = rightFrame;
                        pictureBox.Image = stereoFilter.Apply( leftFrame );
                        pictureBox.Invalidate( );

                        if ( old != null )
                        {
                            old.Dispose( );
                        }
                    }
                    catch
                    {
                    }
                }

                leftFrameIsAvailable.Reset( );
                rightFrameIsAvailable.Reset( );
            }
        }

        private delegate void UpdateWindowSizeCallback( );

        // Update size of the window, so it shows pictures without rescaling
        private void UpdateWindowSize( )
        {
            if ( InvokeRequired )
            {
                Invoke( new UpdateWindowSizeCallback( UpdateWindowSize ) );
            }
            else
            {
                this.Size = new Size( leftFrame.Width + 30, rightFrame.Height + 48 );
                pictureBox.Size = new Size( leftFrame.Width + 2, rightFrame.Height + 2 );
            }
        }
    }
}
