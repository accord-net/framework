using Accord.Imaging;
using Accord.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;

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
            ToByteArray_test_general(PixelFormat.Format32bppArgb, 3, 3, 4 * 9);
        }

        private static void ToByteArray_test_general(PixelFormat pixelFormat, int w, int h, int expected)
        {
            Console.WriteLine($"pixelFormat = {pixelFormat}, w = {w}, h = {h}, expected = {expected}");
            int c = pixelFormat.GetNumberOfChannels();
            Console.WriteLine(c);
            byte[,,] values = (byte[,,])Vector.Range((byte)0, (byte)255).Get(0, c * h * w).Reshape(new[] { h, w, c });
            Console.WriteLine(Accord.Math.Matrix.ToString(values));
            UnmanagedImage image = values.ToBitmap().ToUnmanagedImage();
            //Accord.Imaging.Tools.ToMatrix(image, 0, 255);

            int formatBytes = pixelFormat.GetPixelFormatSizeInBytes();
            byte[] b = image.ToByteArray();
            Console.WriteLine(b.ToString(CSharpMatrixFormatProvider.CurrentCulture));

            System.Diagnostics.Trace.Assert(w * h * formatBytes == b.Length);
            System.Diagnostics.Trace.Assert(expected == b.Length);

            // Reconstruct the original matrix
            UnmanagedImage r = UnmanagedImage.FromByteArray(b, w, h, pixelFormat);
            byte[,,] actual = r.ToManagedImage().ToMatrix((byte)0, (byte)255);
            string a = String.Join(" ", (string[])Accord.Math.Matrix.ToString(actual).DeepFlatten());
            string e = String.Join(" ", (string[])Accord.Math.Matrix.ToString(values).DeepFlatten());
            Console.WriteLine(a);
            Console.WriteLine(b);

            System.Diagnostics.Trace.Assert(e == a);
        }

    }
}
