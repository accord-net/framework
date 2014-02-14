// Accord Control Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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

namespace Accord.Controls
{
    using System.Drawing;
    using System.Windows.Forms;
    using System;
    using AForge.Imaging;

    using Image = System.Drawing.Image;
    using Accord.Imaging.Converters;

    /// <summary>
    ///   Displays images in a similar way to System.Windows.Forms.MessageBox.
    /// </summary>
    /// 
    public partial class ImageBox : Form
    {


        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="image">The image to show.</param>
        /// <param name="imageWidth">The width of the image.</param>
        /// <param name="imageHeight">The height of the image.</param>
        /// 
        public static DialogResult Show(double[] image, int imageWidth, int imageHeight)
        {
            Bitmap bitmap;
            new ArrayToImage(imageWidth, imageHeight).Convert(image, out bitmap);
            return Show(bitmap, PictureBoxSizeMode.AutoSize);
        }

        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="image">The image to show.</param>
        /// 
        public static DialogResult Show(double[,] image)
        {
            Bitmap bitmap;
            new MatrixToImage().Convert(image, out bitmap);
            return Show(bitmap, PictureBoxSizeMode.AutoSize);
        }

        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="image">The image to show.</param>
        /// <param name="imageWidth">The width of the image.</param>
        /// <param name="imageHeight">The height of the image.</param>
        /// <param name="sizeMode">How to display the image inside the image box.</param>
        /// 
        public static DialogResult Show(double[] image, int imageWidth, int imageHeight,
            PictureBoxSizeMode sizeMode)
        {
            Bitmap bitmap;
            new ArrayToImage(imageWidth, imageHeight).Convert(image, out bitmap);
            return Show("Image", bitmap, sizeMode);
        }

        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="image">The image to show.</param>
        /// <param name="imageWidth">The width of the image.</param>
        /// <param name="imageHeight">The height of the image.</param>
        /// <param name="width">The width of the image box.</param>
        /// <param name="height">The height of the image box.</param>
        /// <param name="sizeMode">How to display the image inside the image box.</param>
        /// 
        public static DialogResult Show(double[] image, int imageWidth, int imageHeight,
            PictureBoxSizeMode sizeMode, int width, int height)
        {
            Bitmap bitmap;
            new ArrayToImage(imageWidth, imageHeight).Convert(image, out bitmap);
            return Show("Image", bitmap, sizeMode, width, height);
        }

        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="image">The image to show.</param>
        /// <param name="sizeMode">How to display the image inside the image box.</param>
        /// 
        public static DialogResult Show(double[,] image, PictureBoxSizeMode sizeMode)
        {
            Bitmap bitmap;
            new MatrixToImage().Convert(image, out bitmap);
            return Show("Image", bitmap, sizeMode);
        }

        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="image">The image to show.</param>
        /// <param name="width">The width of the image box.</param>
        /// <param name="height">The height of the image box.</param>
        /// <param name="sizeMode">How to display the image inside the image box.</param>
        /// 
        public static DialogResult Show(double[,] image,
            PictureBoxSizeMode sizeMode, int width, int height)
        {
            return Show("Image", image, sizeMode, width, height);
        }

        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="title">The text to display in the title bar of the image box.</param>
        /// <param name="image">The image to show.</param>
        /// <param name="width">The width of the image box.</param>
        /// <param name="height">The height of the image box.</param>
        /// <param name="sizeMode">How to display the image inside the image box.</param>
        /// 
        public static DialogResult Show(String title, double[,] image,
            PictureBoxSizeMode sizeMode, int width, int height)
        {
            Bitmap bitmap;
            new MatrixToImage().Convert(image, out bitmap);
            return Show(title, bitmap, sizeMode, width, height);
        }

        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="image">The image to show.</param>
        /// 
        public static DialogResult Show(UnmanagedImage image)
        {
            return Show(image, PictureBoxSizeMode.AutoSize);
        }

        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="image">The image to show.</param>
        /// 
        public static DialogResult Show(Image image)
        {
            return Show(image, PictureBoxSizeMode.AutoSize);
        }

        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="image">The image to show.</param>
        /// <param name="width">The width of the image box.</param>
        /// <param name="height">The height of the image box.</param>
        /// 
        public static DialogResult Show(Image image, int width, int height)
        {
            return Show("Image", image, PictureBoxSizeMode.StretchImage, width, height);
        }

        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="image">The image to show.</param>
        /// <param name="sizeMode">How to display the image inside the image box.</param>
        /// <param name="width">The width of the image box.</param>
        /// <param name="height">The height of the image box.</param>
        /// 
        public static DialogResult Show(Image image, PictureBoxSizeMode sizeMode, int width, int height)
        {
            return Show("Image", image, sizeMode, width, height, Color.Black);
        }


        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="image">The image to show.</param>
        /// <param name="sizeMode">How to display the image inside the image box.</param>
        /// 
        public static DialogResult Show(Image image, PictureBoxSizeMode sizeMode)
        {
            return Show("Image", image, sizeMode, 0, 0);
        }

        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="image">The image to show.</param>
        /// <param name="sizeMode">How to display the image inside the image box.</param>
        /// 
        public static DialogResult Show(UnmanagedImage image, PictureBoxSizeMode sizeMode)
        {
            return Show("Image", image, sizeMode, 0, 0);
        }

        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="image">The image to show.</param>
        /// <param name="sizeMode">How to display the image inside the image box.</param>
        /// <param name="backColor">The background color to use in the window. 
        ///   Default is <see cref="Color.Black"/>.</param>
        ///   
        public static DialogResult Show(Image image, PictureBoxSizeMode sizeMode, Color backColor)
        {
            return Show("Image", image, sizeMode, 0, 0, backColor);
        }

        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="title">The text to display in the title bar of the image box.</param>
        /// <param name="image">The image to show.</param>
        /// 
        public static DialogResult Show(string title, Image image)
        {
            return Show(title, image, PictureBoxSizeMode.AutoSize, 0, 0);
        }

        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="title">The text to display in the title bar of the image box.</param>
        /// <param name="image">The image to show.</param>
        /// <param name="sizeMode">How to display the image inside the image box.</param>
        /// 
        public static DialogResult Show(string title, Image image, PictureBoxSizeMode sizeMode)
        {
            return Show(title, image, sizeMode, 0, 0);
        }

        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="title">The text to display in the title bar of the image box.</param>
        /// <param name="image">The image to show.</param>
        /// <param name="sizeMode">How to display the image inside the image box.</param>
        /// 
        public static DialogResult Show(string title, UnmanagedImage image, PictureBoxSizeMode sizeMode)
        {
            return Show(title, image, sizeMode, 0, 0);
        }

        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="title">The text to display in the title bar of the image box.</param>
        /// <param name="image">The image to show.</param>
        /// <param name="sizeMode">How to display the image inside the image box.</param>
        /// <param name="width">The width of the image box.</param>
        /// <param name="height">The height of the image box.</param>
        /// 
        public static DialogResult Show(string title, Image image, PictureBoxSizeMode sizeMode, int width, int height)
        {
            return Show(title, image, sizeMode, width, height, Color.Black);
        }

        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="title">The text to display in the title bar of the image box.</param>
        /// <param name="image">The image to show.</param>
        /// <param name="sizeMode">How to display the image inside the image box.</param>
        /// <param name="width">The width of the image box.</param>
        /// <param name="height">The height of the image box.</param>
        /// 
        public static DialogResult Show(string title, UnmanagedImage image, PictureBoxSizeMode sizeMode, int width, int height)
        {
            return Show(title, image, sizeMode, width, height, Color.Black);
        }

        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="title">The text to display in the title bar of the image box.</param>
        /// <param name="image">The image to show.</param>
        /// <param name="sizeMode">How to display the image inside the image box.</param>
        /// <param name="width">The width of the image box.</param>
        /// <param name="height">The height of the image box.</param>
        /// <param name="backColor">The background color to use in the window. 
        ///   Default is <see cref="Color.Black"/>.</param>
        /// 
        public static DialogResult Show(string title, Image image, PictureBoxSizeMode sizeMode, int width, int height, Color backColor)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            DialogResult result;

            if (width == 0 && height == 0)
            {
                switch (sizeMode)
                {
                    case PictureBoxSizeMode.AutoSize:
                        width = image.Width;
                        height = image.Height;
                        break;

                    case PictureBoxSizeMode.CenterImage:
                    case PictureBoxSizeMode.Normal:
                    case PictureBoxSizeMode.StretchImage:
                    case PictureBoxSizeMode.Zoom:
                        width = 320;
                        height = 240;
                        break;
                }
            }

            using (ImageBox box = new ImageBox())
            {
                box.SuspendLayout();
                box.Text = title;
                box.BackColor = backColor;
                box.ClientSize = new Size(width, height);
                box.pictureBox.SizeMode = sizeMode;
                box.pictureBox.Image = image;
                box.ResumeLayout(false);

                result = box.ShowDialog();
            }

            return result;
        }

        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="title">The text to display in the title bar of the image box.</param>
        /// <param name="image">The image to show.</param>
        /// <param name="sizeMode">How to display the image inside the image box.</param>
        /// <param name="width">The width of the image box.</param>
        /// <param name="height">The height of the image box.</param>
        /// <param name="backColor">The background color to use in the window. 
        ///   Default is <see cref="Color.Black"/>.</param>
        /// 
        public static DialogResult Show(string title, UnmanagedImage image, PictureBoxSizeMode sizeMode, int width, int height, Color backColor)
        {
            return Show(title, image.ToManagedImage(), sizeMode, width, height, backColor);
        }


        private ImageBox()
        {
            InitializeComponent();
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.PreviewKeyDown"/> event.
        /// </summary>
        /// 
        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();

            base.OnPreviewKeyDown(e);
        }

    }
}
