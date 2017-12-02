using Accord.Imaging.Converters;
using Accord.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;

using RotateNearestNeighbor = Accord.Imaging.Filters.RotateNearestNeighbor;

namespace Accord.Tests.Imaging
{
    public static class ImageUtils
    {

        public static bool RotateTest8bpp(IFilter filter, Bitmap input, Bitmap output)
        {
            var itm = new ImageToMatrix();

            // Test directly
            double[,] actual;
            itm.Convert(filter.Apply(input), out actual);

            double[,] expected;
            itm.Convert(output, out expected);

            if (!actual.IsEqual(expected))
                return false;

            // Rotate and re-test
            var rotate = new RotateNearestNeighbor(90, false);
            input = rotate.Apply(input);
            output = rotate.Apply(output);

            itm.Convert(filter.Apply(input), out actual);
            itm.Convert(output, out expected);

            return actual.IsEqual(expected);
        }

        public static bool RotateTest32bpp(IFilter filter, Bitmap input, Bitmap output)
        {
            var itm = new ImageToMatrix();

            // Test directly
            Color[,] actual;
            itm.Convert(filter.Apply(input), out actual);

            Color[,] expected;
            itm.Convert(output, out expected);

            if (!actual.IsEqual(expected))
                return false;

            // Rotate and re-test
            var rotate = new RotateNearestNeighbor(90, keepSize: false);
            input = rotate.Apply(input);
            output = rotate.Apply(output);

            itm.Convert(filter.Apply(input), out actual);
            itm.Convert(output, out expected);

            return actual.IsEqual(expected);
        }
    }
}
