// Image Viewer sample application
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2006-2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using AForge.Imaging.Formats;

namespace ImageViewer
{
    // Main form's class
    public partial class MainForm : Form
    {
        // Class constructor
        public MainForm( )
        {
            InitializeComponent( );
        }

        // Exit from application
        private void exitToolStripMenuItem_Click( object sender, EventArgs e )
        {
            Application.Exit( );
        }

        // Open image file
        private void openToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( openFileDialog.ShowDialog( ) == DialogResult.OK )
            {
                try
                {
                    ImageInfo imageInfo = null;

                    pictureBox.Image = ImageDecoder.DecodeFromFile( openFileDialog.FileName, out imageInfo );

                    propertyGrid.SelectedObject = imageInfo;
                    propertyGrid.ExpandAllGridItems( );
                }
                catch ( NotSupportedException ex )
                {
                    MessageBox.Show( "Image format is not supported: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error );
                }
                catch ( ArgumentException ex )
                {
                    MessageBox.Show( "Invalid image: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error );
                }
                catch
                {
                    MessageBox.Show( "Failed loading the image", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error );
                }
            }
        }
    }
}
