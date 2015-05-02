// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2010
// contacts@aforgenet.com
//

namespace AForge.Imaging.ColorReduction
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using AForge.Imaging;

    // Color cube used by Median Cut color quantization algorithm
    internal class MedianCutCube
    {
        private List<Color> colors;

        private readonly byte minR, maxR;
        private readonly byte minG, maxG;
        private readonly byte minB, maxB;

        private Color? cubeColor = null;

        // Length of the "red side" of the cube
        public int RedSize
        {
            get { return maxR - minR; }
        }

        // Length of the "green size" of the cube
        public int GreenSize
        {
            get { return maxG - minG; }
        }

        // Length of the "blue size" of the cube
        public int BlueSize
        {
            get { return maxB - minB; }
        }

        // Mean cube's color
        public Color Color
        {
            get
            {
                if ( cubeColor == null )
                {
                    int red = 0, green = 0, blue = 0;

                    foreach ( Color color in colors )
                    {
                        red   += color.R;
                        green += color.G;
                        blue  += color.B;
                    }

                    int colorsCount = colors.Count;

                    if ( colorsCount != 0 )
                    {
                        red   /= colorsCount;
                        green /= colorsCount;
                        blue  /= colorsCount;
                    }

                    cubeColor = Color.FromArgb( red, green, blue );
                }

                return cubeColor.Value;
            }
        }

        public MedianCutCube( List<Color> colors )
        {
            this.colors = colors;

            // get min/max values for each RGB component of specified colors
            minR = minG = minB = 255;
            maxR = maxG = maxB = 0;

            foreach ( Color color in colors )
            {
                if ( color.R < minR )
                    minR = color.R;
                if ( color.R > maxR )
                    maxR = color.R;

                if ( color.G < minG )
                    minG = color.G;
                if ( color.G > maxG )
                    maxG = color.G;

                if ( color.B < minB )
                    minB = color.B;
                if ( color.B > maxB )
                    maxB = color.B;
            }
        }

        // Split the cube into 2 smaller cubes using the specified color side for splitting
        public void SplitAtMedian( int rgbComponent, out MedianCutCube cube1, out MedianCutCube cube2 )
        {
            switch ( rgbComponent )
            {
                case RGB.R:
                    colors.Sort( new RedComparer( ) );
                    break;
                case RGB.G:
                    colors.Sort( new GreenComparer( ) );
                    break;
                case RGB.B:
                    colors.Sort( new BlueComparer( ) );
                    break;
            }

            int median = colors.Count / 2;

            cube1 = new MedianCutCube( colors.GetRange( 0, median ) );
            cube2 = new MedianCutCube( colors.GetRange( median, colors.Count - median ) );
        }

        #region Different comparers used for sorting colors by different components
        private class RedComparer : IComparer<Color>
        {
            public int Compare( Color c1, Color c2 )
            {
                return c1.R.CompareTo( c2.R );
            }
        }

        private class GreenComparer : IComparer<Color>
        {
            public int Compare( Color c1, Color c2 )
            {
                return c1.G.CompareTo( c2.G );
            }
        }

        private class BlueComparer : IComparer<Color>
        {
            public int Compare( Color c1, Color c2 )
            {
                return c1.B.CompareTo( c2.B );
            }
        }
        #endregion
    }
}
