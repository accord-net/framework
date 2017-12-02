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
    using Accord.Math;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    ///   BitmapSource to jagged array converter.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class converts images to single or jagged arrays of
    ///   either double-precision or single-precision floating-point
    ///   values.
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This example converts a 16x16 Bitmap image into
    ///   a double[] array with values between 0 and 1.</para>
    ///   
    /// <code>
    /// // Obtain a 16x16 bitmap image
    /// // BitmapSource image = ...
    /// 
    /// // Create the converter to convert the image to an
    /// //   array containing only values between 0 and 1 
    /// var conv = new ImageSourceToArray(min: 0, max: 1);
    /// 
    /// // Convert the image and store it in the array
    /// double[] array; conv.Convert(image, out array);
    /// 
    /// // Show the array on screen
    /// ImageBox.Show(array, 16, 16, PictureBoxSizeMode.Zoom);    /// </code>
    /// 
    /// <para>
    ///   The resulting image is shown below.</para>
    /// 
    /// <img src="..\images\image-to-matrix.png" />
    /// 
    /// </example>
    /// 
    public class BitmapSourceToArray :
        IConverter<BitmapSource, byte[]>,
        IConverter<BitmapSource, byte[][]>,
        IConverter<BitmapSource, double[]>,
        IConverter<BitmapSource, double[][]>,
        IConverter<BitmapSource, float[]>,
        IConverter<BitmapSource, float[][]>
    {

        /// <summary>
        ///   Gets or sets the channel to be extracted.
        /// </summary>
        /// 
        public int Channel { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ImageToArray"/> class.
        /// </summary>
        /// 
        /// <param name="channel">The channel to extract. Default is 0.</param>
        ///   
        public BitmapSourceToArray(int channel)
        {
            this.Channel = channel;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ImageToArray"/> class.
        /// </summary>
        /// 
        public BitmapSourceToArray()
        {
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
        public void Convert(BitmapSource input, out double[] output)
        {
            float[] f;
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
        public void Convert(BitmapSource input, out float[][] output)
        {
            var c = new FormatConvertedBitmap(input, PixelFormats.Rgba128Float, null, 1.0);
            int width = c.PixelWidth;
            int height = c.PixelHeight;
            var buffer = new float[width * height, 4];
            c.CopyPixels(buffer);
            output = buffer.ToJagged();
        }


        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(BitmapSource input, out float[] output)
        {
            var c = new FormatConvertedBitmap(input, PixelFormats.Rgba128Float, null, 1.0);
            int width = c.PixelWidth;
            int height = c.PixelHeight;
            int stride = c.GetStride();
            float[,] f = new float[width * height, 4];
            c.CopyPixels(f);
            output = f.GetColumn(Channel);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(BitmapSource input, out byte[] output)
        {
            var c = new FormatConvertedBitmap(input, PixelFormats.Gray8, null, 1.0);
            int width = c.PixelWidth;
            int height = c.PixelHeight;
            int stride = c.GetStride();
            output = new byte[width * height];
            c.CopyPixels(output, stride, 0);
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
            int width = input.PixelWidth;
            int height = input.PixelHeight;
            int stride = input.GetStride();

            var c = new FormatConvertedBitmap(input, PixelFormats.Bgr32, null, 1.0);
            var buffer = new byte[width * height, 4];
            c.CopyPixels(buffer, stride, 0);
            output = buffer.ToJagged();
        }

    }
}
