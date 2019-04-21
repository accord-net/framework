using Accord.Imaging;
using Accord.Imaging.Converters;
using Accord.Imaging.Filters;
using Accord.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
            double[,] diag = Matrix.Magic(5);

            Bitmap input; new MatrixToImage().Convert(diag, out input);

            // Create a new Variance filter
            Variance filter = new Variance();

            // Apply the filter
            Bitmap output = filter.Apply(input);

            double[,] actual;

            new ImageToMatrix().Convert(output, out actual);

            string str = actual.ToString(CSharpMatrixFormatProvider.InvariantCulture);
            Console.WriteLine(str);

            double[,] expected =
            {
                { 0, 0, 0, 0, 0 },
                { 0.0941176470588235, 0.545098039215686, 0.396078431372549, 0.376470588235294, 0.192156862745098 },
                { 0.298039215686275, 0.376470588235294, 0.27843137254902, 0.211764705882353, 0.133333333333333 },
                { 0.317647058823529, 0.203921568627451, 0.2, 0.16078431372549, 0.109803921568627 },
                { 0.0509803921568627, 0.109803921568627, 0.16078431372549, 0.2, 0.203921568627451 }
            };
        }

    }
}
