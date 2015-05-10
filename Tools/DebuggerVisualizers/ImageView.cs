// AForge debugging visualizers
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;

namespace AForge.DebuggerVisualizers
{
    public partial class ImageView : Form
    {
        public ImageView( )
        {
            InitializeComponent( );
        }

        private void ImageView_Resize( object sender, EventArgs e )
        {
            UpdateViewSize( );
        }

        private void ImageView_Load( object sender, EventArgs e )
        {
            if ( pictureBox.Image != null )
            {
                // if image was already set by the time of form loading, update form's size
                int width  = System.Math.Max( 64, System.Math.Min( 800, pictureBox.Image.Width ) );
                int height = System.Math.Max( 64, System.Math.Min( 600, pictureBox.Image.Height ) );
                this.Size = new Size( width + 80, height + 155 );
            }
        }

        public void SetImage( Image image )
        {
            pictureBox.Image = image;

            if ( image == null )
            {
                Text = "Image not set";
                saveButton.Enabled = false;
                clipboardButton.Enabled = false;
            }
            else
            {
                Text = string.Format( "Width: {0}, Height: {1}", image.Width, image.Height );
                saveButton.Enabled = true;
                clipboardButton.Enabled = true;
            }

            UpdateViewSize( );
        }

        private void UpdateViewSize( )
        {
            pictureBox.SuspendLayout( );

            int width = 160;
            int height = 120;

            if ( pictureBox.Image != null )
            {
                width  = pictureBox.Image.Width + 2;
                height = pictureBox.Image.Height + 2;
            }

            int x = ( width > imagePanel.ClientSize.Width ) ? 0 : ( imagePanel.ClientSize.Width - width ) / 2;
            int y = ( height > imagePanel.ClientSize.Height ) ? 0 : ( imagePanel.ClientSize.Height - height) / 2;

            pictureBox.Size = new Size( width, height );
            pictureBox.Location = new System.Drawing.Point( x, y );

            pictureBox.ResumeLayout( );
        }

        // Put image into clipboard
        private void clipboardButton_Click( object sender, EventArgs e )
        {
            if ( pictureBox.Image != null )
            {
                Clipboard.SetDataObject( pictureBox.Image );
            }
        }

        // Save image to file
        private void saveButton_Click( object sender, EventArgs e )
        {
            if ( pictureBox.Image != null )
            {
                if ( saveFileDialog.ShowDialog( ) == DialogResult.OK )
                {
                    ImageFormat format = ImageFormat.Jpeg;

                    // resolve file format
                    switch ( Path.GetExtension( saveFileDialog.FileName ).ToLower( ) )
                    {
                        case ".jpg":
                            format = ImageFormat.Jpeg;
                            break;
                        case ".bmp":
                            format = ImageFormat.Bmp;
                            break;
                        case ".png":
                            format = ImageFormat.Png;
                            break;
                        default:
                            MessageBox.Show( this, "Unsupported image format was specified", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error );
                            return;
                    }

                    // save the image
                    try
                    {
                        pictureBox.Image.Save( saveFileDialog.FileName, format );
                    }
                    catch ( Exception ex )
                    {
                        MessageBox.Show( this, "Failed writing image file.\r\n\r\n" + ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error );
                    }
                }
            }
        }
    }
}
