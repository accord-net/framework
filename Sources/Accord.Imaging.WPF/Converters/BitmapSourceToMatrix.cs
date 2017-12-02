// Accord Imaging Library
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

namespace Accord.Imaging.Converters
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using Accord.Imaging;
    using Accord.Math;
    using System;
    using System.Windows.Media.Imaging;
    using System.Windows.Media;

    /// <summary>
    ///   BitmapSource to multidimensional matrix converter.
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
    /// // BitmapSource image = ...
    ///
    /// // Create the converter to convert the image to a
    /// //  matrix containing only values between 0 and 1 
    /// var conv = new BitmapSourceToMatrix(min: 0, max: 1);
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
    public class BitmapSourceToMatrix :
        IConverter<BitmapSource, double[,]>,
        IConverter<BitmapSource, double[][]>,
        IConverter<BitmapSource, float[,]>,
        IConverter<BitmapSource, float[][]>,
        IConverter<BitmapSource, byte[,]>,
        IConverter<BitmapSource, byte[][]>,
        IConverter<BitmapSource, double[,,]>,
        IConverter<BitmapSource, double[][][]>,
        IConverter<BitmapSource, float[,,]>,
        IConverter<BitmapSource, float[][][]>,
        IConverter<BitmapSource, byte[,,]>,
        IConverter<BitmapSource, byte[][][]>
    {

        /// <summary>
        ///   Gets or sets the horizontal DPI of the image stored in the double array. Default is 96.
        /// </summary>
        /// 
        public int DpiX { get; set; }

        /// <summary>
        ///   Gets or sets the vertical DPI of the image stored in the double array. Default is 96.
        /// </summary>
        /// 
        public int DpiY { get; set; }

        /// <summary>
        ///   Gets or sets the channel to be extracted.
        /// </summary>
        /// 
        public int Channel { get; set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="ImageToMatrix"/> class.
        /// </summary>
        ///   
        public BitmapSourceToMatrix()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ImageToMatrix"/> class.
        /// </summary>
        /// 
        /// <param name="channel">The channel to extract. Default is 0.</param>
        ///   
        public BitmapSourceToMatrix(int channel)
        {
            this.Channel = channel;
        }


        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(BitmapSource input, out double[,] output)
        {
            float[,] f;
            Convert(input, out f);
            output = f.ToDouble();
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(BitmapSource input, out double[][] output)
        {
            float[][] f;
            Convert(input, out f);
            output = f.ToDouble();
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(BitmapSource input, out float[,] output)
        {
            float[,,] f;
            Convert(input, out f);
            output = f.GetPlane(Channel); 
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(BitmapSource input, out float[][] output)
        {
            float[,] f;
            Convert(input, out f);
            output = f.ToJagged();
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(BitmapSource input, out byte[,] output)
        {
            int width = input.PixelWidth;
            int height = input.PixelHeight;

            var c = new FormatConvertedBitmap(input, PixelFormats.Gray8, null, 1.0);
            output = new byte[width, height];
            c.CopyPixels(output);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(BitmapSource input, out byte[][] output)
        {
            byte[,] f;
            Convert(input, out f);
            output = f.ToJagged();
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(BitmapSource input, out double[,,] output)
        {
            float[,,] f;
            Convert(input, out f);
            output = f.ToDouble();
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(BitmapSource input, out double[][][] output)
        {
            float[][][] f;
            Convert(input, out f);
            output = f.ToDouble();
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(BitmapSource input, out float[,,] output)
        {
            var c = new FormatConvertedBitmap(input, PixelFormats.Rgba128Float, null, 1.0);
            int width = c.PixelWidth;
            int height = c.PixelHeight;
            int stride = c.GetStride();
            output = new float[width, height, 4];
            c.CopyPixels(output);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(BitmapSource input, out float[][][] output)
        {
            float[,,] f;
            Convert(input, out f);
            output = f.ToJagged();
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(BitmapSource input, out byte[,,] output)
        {
            int width = input.PixelWidth;
            int height = input.PixelHeight;

            var c = new FormatConvertedBitmap(input, PixelFormats.Bgra32, null, 1.0);
            output = new byte[width, height, 4];
            c.CopyPixels(output);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(BitmapSource input, out byte[][][] output)
        {
            byte[,,] f;
            Convert(input, out f);
            output = f.ToJagged();
        }

    }
}
