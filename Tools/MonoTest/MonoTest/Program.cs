using Accord.Imaging;
using Accord.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonoTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            TestImaging();
        }


        private static void TestImaging()
        {
            ToByteArray_test_general(PixelFormat.Format8bppIndexed, 3, 3, 9);
            ToByteArray_test_general(PixelFormat.Format8bppIndexed, 4, 5, 20);
            ToByteArray_test_general(PixelFormat.Format8bppIndexed, 5, 4, 20);
            ToByteArray_test_general(PixelFormat.Format8bppIndexed, 17, 13, 221);
            ToByteArray_test_general(PixelFormat.Format32bppArgb, 3, 3, 4 * 9);
            ToByteArray_test_general(PixelFormat.Format32bppArgb, 4, 5, 4 * 20);
            ToByteArray_test_general(PixelFormat.Format32bppArgb, 5, 4, 4 * 20);
            ToByteArray_test_general(PixelFormat.Format24bppRgb, 3, 3, 3 * 9);
            ToByteArray_test_general(PixelFormat.Format24bppRgb, 4, 5, 3 * 20);
            ToByteArray_test_general(PixelFormat.Format24bppRgb, 5, 4, 3 * 20);
        }

        private static void ToByteArray_test_general(PixelFormat pixelFormat, int w, int h, int expected)
        {
            int c = pixelFormat.GetNumberOfChannels();
            Console.WriteLine(c);
            byte[,,] values = (byte[,,])Vector.Range((byte)0, (byte)255).Get(0, h * w).Reshape(new[] { h, w, c });
            Console.WriteLine(Accord.Math.Matrix.ToString(values));
            UnmanagedImage image = values.ToBitmap().ToUnmanagedImage();
            Accord.Imaging.Tools.ToMatrix(image, 0, 255);

            int formatBytes = pixelFormat.GetPixelFormatSizeInBytes();
            byte[] b = image.ToByteArray();
            Console.WriteLine(b.ToString(CSharpMatrixFormatProvider.CurrentCulture));

            System.Diagnostics.Debug.Assert(w * h * formatBytes == b.Length);
            System.Diagnostics.Debug.Assert(expected == b.Length);

            // Reconstruct the original matrix
            UnmanagedImage r = UnmanagedImage.FromByteArray(b, w, h, pixelFormat);
            byte[,,] actual = r.ToManagedImage().ToMatrix((byte)0, (byte)255);
            Console.WriteLine(Accord.Math.Matrix.ToString(actual));
            Console.WriteLine(Accord.Math.Matrix.ToString(actual));

            System.Diagnostics.Debug.Assert(values.IsEqual(actual));
        }

    }
}
