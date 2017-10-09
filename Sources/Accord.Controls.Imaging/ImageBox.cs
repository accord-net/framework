// Accord Control Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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
    using Accord.Imaging;

    using Image = System.Drawing.Image;
    using Accord.Imaging.Converters;
    using System.Drawing.Imaging;
    using System.Threading;

    /// <summary>
    ///   Displays images in a similar way to System.Windows.Forms.MessageBox.
    /// </summary>
    /// 
    public partial class ImageBox : Form
    {
        private Thread formThread;


        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="image">The image to show.</param>
        /// <param name="imageWidth">The width of the image.</param>
        /// <param name="imageHeight">The height of the image.</param>
        /// 
        public static ImageBox Show(double[] image, int imageWidth, int imageHeight)
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
        public static ImageBox Show(double[,] image)
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
        public static ImageBox Show(double[] image, int imageWidth, int imageHeight,
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
        public static ImageBox Show(double[] image, int imageWidth, int imageHeight,
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
        public static ImageBox Show(double[,] image, PictureBoxSizeMode sizeMode)
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
        public static ImageBox Show(double[,] image,
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
        public static ImageBox Show(String title, double[,] image,
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
        public static ImageBox Show(UnmanagedImage image)
        {
            return Show(image, PictureBoxSizeMode.AutoSize);
        }

        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="image">The image to show.</param>
        /// 
        public static ImageBox Show(Image image)
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
        public static ImageBox Show(Image image, int width, int height)
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
        public static ImageBox Show(Image image, PictureBoxSizeMode sizeMode, int width, int height)
        {
            return show("Image", image, sizeMode, width, height, Color.Black);
        }


        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="image">The image to show.</param>
        /// <param name="sizeMode">How to display the image inside the image box.</param>
        /// 
        public static ImageBox Show(Image image, PictureBoxSizeMode sizeMode)
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
        public static ImageBox Show(UnmanagedImage image, PictureBoxSizeMode sizeMode)
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
        public static ImageBox Show(Image image, PictureBoxSizeMode sizeMode, Color backColor)
        {
            return show("Image", image, sizeMode, 0, 0, backColor);
        }

        /// <summary>
        ///   Displays an image on the screen.
        /// </summary>
        /// 
        /// <param name="title">The text to display in the title bar of the image box.</param>
        /// <param name="image">The image to show.</param>
        /// 
        public static ImageBox Show(string title, Image image)
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
        public static ImageBox Show(string title, Image image, PictureBoxSizeMode sizeMode)
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
        public static ImageBox Show(string title, UnmanagedImage image,
            PictureBoxSizeMode sizeMode)
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
        public static ImageBox Show(string title, Image image,
            PictureBoxSizeMode sizeMode, int width, int height)
        {
            return show(title, image, sizeMode, width, height, Color.Black);
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
        public static ImageBox Show(string title, UnmanagedImage image,
            PictureBoxSizeMode sizeMode, int width, int height)
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
        public static ImageBox Show(string title, UnmanagedImage image,
            PictureBoxSizeMode sizeMode, int width, int height, Color backColor)
        {
            return show(title, image.ToManagedImage(), sizeMode, width, height, backColor);
        }


        private static ImageBox show(string title, Image image,
            PictureBoxSizeMode sizeMode, int width, int height, Color backColor)
        {
            ImageBox form = null;
            Thread formThread = null;

            AutoResetEvent stopWaitHandle = new AutoResetEvent(false);

            GetDimensions(image, sizeMode, ref width, ref height);

            formThread = new Thread(() =>
            {
                Accord.Controls.Tools.ConfigureWindowsFormsApplication();

                // Show control in a form
                form = new ImageBox();
                form.Text = title ?? "Image";
                form.formThread = formThread;
                form.Text = title;
                form.BackColor = backColor;
                form.ClientSize = new Size(width, height);
                form.pictureBox.SizeMode = sizeMode;
                form.pictureBox.Image = image;
                form.ResumeLayout(false);

                stopWaitHandle.Set();

                Application.Run(form);
            });

            formThread.SetApartmentState(ApartmentState.STA);

            formThread.Start();

            stopWaitHandle.WaitOne();

            return form;
        }

        private static void GetDimensions(Image image, PictureBoxSizeMode sizeMode, ref int width, ref int height)
        {
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
        }

        /// <summary>
        ///   Sets the window title of the data grid box.
        ///   
        /// </summary>
        /// <param name="text">The desired title text for the window.</param>
        /// 
        /// <returns>This instance, for fluent programming.</returns>
        /// 
        public ImageBox SetTitle(string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetTitle(text)));
                return this;
            }

            this.Text = text;

            return this;
        }

        /// <summary>
        ///   Holds the execution until the window has been closed.
        /// </summary>
        /// 
        public void Hold()
        {
            if (Thread.CurrentThread == formThread)
                return;

            this.SetTitle(this.Text + " [on hold]");

            formThread.Join();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ImageBox"/> class.
        /// </summary>
        /// 
        protected ImageBox()
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


        private void centeredToolStripMenuItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.pictureBox.SizeMode = (PictureBoxSizeMode)centeredToolStripMenuItem.ComboBox.SelectedValue;
        }

        private void ImageBox_Load(object sender, EventArgs e)
        {
            var modes = Enum.GetValues(typeof(PictureBoxSizeMode));

            var currentMode = pictureBox.SizeMode;
            this.centeredToolStripMenuItem.ComboBox.BindingContext = this.BindingContext;
            this.centeredToolStripMenuItem.ComboBox.DataSource = modes;
            this.centeredToolStripMenuItem.ComboBox.SelectedItem = currentMode;
        }

        private void zoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog(this);
        }

        private void saveFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ImageFormat format = Accord.Imaging.Tools.GetFormat(saveFileDialog1.FileName);

            using (var file = saveFileDialog1.OpenFile())
                this.pictureBox.Image.Save(file, format);
        }

    }
}
