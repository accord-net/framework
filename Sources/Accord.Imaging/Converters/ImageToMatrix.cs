// Accord Imaging Library
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

namespace Accord.Imaging.Converters
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge.Imaging;

    /// <summary>
    ///   Bitmap to multidimensional matrix converter.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class converts images to multidimensional matrices of
    ///   either double-precision or single-precision floating-point
    ///   values.
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This example converts a 16x16 Bitmap image into
    ///   a double[,] array with values between 0 and 1.</para>
    ///   
    /// <code>
    /// // Obtain an image
    /// // Bitmap image = ...
    ///
    /// // Show on screen
    /// ImageBox.Show(image, PictureBoxSizeMode.Zoom);
    ///
    /// // Create the converter to convert the image to a
    /// //  matrix containing only values between 0 and 1 
    /// ImageToMatrix conv = new ImageToMatrix(min: 0, max: 1);
    ///
    /// // Convert the image and store it in the matrix
    /// double[,] matrix; conv.Convert(image, out matrix);
    ///
    /// // Show the matrix on screen as an image
    /// ImageBox.Show(matrix, PictureBoxSizeMode.Zoom);
    /// </code>
    /// <para>
    ///   The resulting image is shown below.</para>
    /// 
    /// <img src="..\images\image-to-matrix.png" />
    /// 
    /// <para>
    ///   Additionally, the image can also be shown in alternative
    ///   representations such as text or data tables.
    /// </para>
    /// 
    /// <code>
    /// // Show the matrix on screen as a .NET multidimensional array
    /// MessageBox.Show(matrix.ToString(CSharpMatrixFormatProvider.InvariantCulture));
    ///
    /// // Show the matrix on screen as a table
    /// DataGridBox.Show(matrix, nonBlocking: true)
    ///     .SetAutoSizeColumns(DataGridViewAutoSizeColumnsMode.Fill)
    ///     .SetAutoSizeRows(DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders)
    ///     .SetDefaultFontSize(5)
    ///    .WaitForClose();
    /// </code>
    /// 
    ///  <para>
    ///   The resulting images are shown below.</para>
    ///   
    /// <img src="..\images\image-to-matrix-string.png" />
    /// <img src="..\images\image-to-matrix-table.png" />
    /// 
    /// </example>
    /// 
    public class ImageToMatrix :
        IConverter<Bitmap, double[,]>,
        IConverter<UnmanagedImage, double[,]>,
        IConverter<Bitmap, float[,]>,
        IConverter<UnmanagedImage, float[,]>,
        IConverter<Bitmap, byte[,]>,
        IConverter<UnmanagedImage, byte[,]>
    {

        /// <summary>
        ///   Gets or sets the maximum double value in the
        ///   double array associated with the brightest color.
        /// </summary>
        /// 
        public double Max { get; set; }

        /// <summary>
        ///   Gets or sets the minimum double value in the
        ///   double array associated with the darkest color.
        /// </summary>
        /// 
        public double Min { get; set; }

        /// <summary>
        ///   Gets or sets the channel to be extracted.
        /// </summary>
        /// 
        public int Channel { get; set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="ImageToMatrix"/> class.
        /// </summary>
        /// 
        /// <param name="min">
        ///   The minimum double value in the double array
        ///   associated with the darkest color. Default is 0.
        /// </param>
        /// <param name="max">
        ///   The maximum double value in the double array
        ///   associated with the brightest color. Default is 1.
        /// </param>
        /// <param name="channel">The channel to extract. Default is 0.</param>
        ///   
        public ImageToMatrix(double min, double max, int channel)
        {
            this.Min = min;
            this.Max = max;
            this.Channel = channel;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ImageToMatrix"/> class.
        /// </summary>
        /// 
        public ImageToMatrix() : this(0, 1) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ImageToMatrix"/> class.
        /// </summary>
        /// 
        /// <param name="min">
        ///   The minimum double value in the double array
        ///   associated with the darkest color. Default is 0.
        /// </param>
        /// <param name="max">
        ///   The maximum double value in the double array
        ///   associated with the brightest color. Default is 1.
        /// </param>
        ///   
        public ImageToMatrix(double min, double max) : this(min, max, 0) { }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(Bitmap input, out double[,] output)
        {
            BitmapData bitmapData = input.LockBits(new Rectangle(0, 0, input.Width, input.Height),
                ImageLockMode.ReadOnly, input.PixelFormat);

            Convert(new UnmanagedImage(bitmapData), out output);

            input.UnlockBits(bitmapData);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(Bitmap input, out float[,] output)
        {
            BitmapData bitmapData = input.LockBits(new Rectangle(0, 0, input.Width, input.Height),
                ImageLockMode.ReadOnly, input.PixelFormat);

            Convert(new UnmanagedImage(bitmapData), out output);

            input.UnlockBits(bitmapData);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(Bitmap input, out byte[,] output)
        {
            BitmapData bitmapData = input.LockBits(new Rectangle(0, 0, input.Width, input.Height),
                ImageLockMode.ReadOnly, input.PixelFormat);

            Convert(new UnmanagedImage(bitmapData), out output);

            input.UnlockBits(bitmapData);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public unsafe void Convert(UnmanagedImage input, out double[,] output)
        {
            int width = input.Width;
            int height = input.Height;
            int pixelSize = Bitmap.GetPixelFormatSize(input.PixelFormat) / 8;
            int offset = input.Stride - input.Width * pixelSize;

            output = new double[height, width];

            fixed (double* ptrData = output)
            {
                double* dst = ptrData;
                byte* src = (byte*)input.ImageData.ToPointer() + Channel;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, src += pixelSize, dst++)
                        *dst = Accord.Math.Tools.Scale(0, 255, Min, Max, *src);
                    src += offset;
                }
            }
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(UnmanagedImage input, out float[,] output)
        {
            int width = input.Width;
            int height = input.Height;
            int pixelSize = Bitmap.GetPixelFormatSize(input.PixelFormat) / 8;
            int offset = input.Stride - input.Width * pixelSize;

            output = new float[height, width];

            float min = (float)Min;
            float max = (float)Max;

            unsafe
            {
                fixed (float* ptrData = output)
                {
                    float* dst = ptrData;
                    byte* src = (byte*)input.ImageData.ToPointer() + Channel;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, src += pixelSize, dst++)
                            *dst = Accord.Math.Tools.Scale(0, 255, min, max, *src);
                        src += offset;
                    }
                }
            }
        }

        /// <summary>
        ///   Converts an image from one representation to another. When
        ///   converting to byte, the <see cref="Max"/> and <see cref="Min"/>
        ///   are ignored.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(UnmanagedImage input, out byte[,] output)
        {
            int width = input.Width;
            int height = input.Height;
            int pixelSize = Bitmap.GetPixelFormatSize(input.PixelFormat) / 8;
            int offset = input.Stride - input.Width * pixelSize;

            output = new byte[height, width];

            unsafe
            {
                fixed (byte* ptrData = output)
                {
                    byte* dst = ptrData;
                    byte* src = (byte*)input.ImageData.ToPointer() + Channel;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, src += pixelSize, dst++)
                            *dst = *src;
                        src += offset;
                    }
                }
            }
        }

    }
}
