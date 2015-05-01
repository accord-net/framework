// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2007-2010
// andrew.kirillov@aforgenet.com
//

using System;
using System.Collections.Generic;
using AForge;

namespace AForge.Imaging.Filters
{
    // The code is based on the code from the below link, which shows
    // the idea of mapping any quadrilateral to any other quadrilateral
    // http://threeblindmiceandamonkey.com/?p=16

    internal static class QuadTransformationCalcs
    {
        private const double TOLERANCE = 1e-13;

        // Caclculates determinant of a 2x2 matrix
        private static double Det2( double a, double b, double c, double d )
        {
            return ( a * d - b * c );
        }

        // Multiply two 3x3 matrices
        private static double[,] MultiplyMatrix( double[,] a, double[,] b )
        {
            double[,] c = new double[3, 3];

            c[0, 0] = a[0, 0] * b[0, 0] + a[0, 1] * b[1, 0] + a[0, 2] * b[2, 0];
            c[0, 1] = a[0, 0] * b[0, 1] + a[0, 1] * b[1, 1] + a[0, 2] * b[2, 1];
            c[0, 2] = a[0, 0] * b[0, 2] + a[0, 1] * b[1, 2] + a[0, 2] * b[2, 2];
            c[1, 0] = a[1, 0] * b[0, 0] + a[1, 1] * b[1, 0] + a[1, 2] * b[2, 0];
            c[1, 1] = a[1, 0] * b[0, 1] + a[1, 1] * b[1, 1] + a[1, 2] * b[2, 1];
            c[1, 2] = a[1, 0] * b[0, 2] + a[1, 1] * b[1, 2] + a[1, 2] * b[2, 2];
            c[2, 0] = a[2, 0] * b[0, 0] + a[2, 1] * b[1, 0] + a[2, 2] * b[2, 0];
            c[2, 1] = a[2, 0] * b[0, 1] + a[2, 1] * b[1, 1] + a[2, 2] * b[2, 1];
            c[2, 2] = a[2, 0] * b[0, 2] + a[2, 1] * b[1, 2] + a[2, 2] * b[2, 2];

            return c;
        }

        // Calculates adjugate 3x3 matrix
        private static double[,] AdjugateMatrix( double[,] a )
        {
            double[,] b = new double[3, 3];
            b[0, 0] = Det2( a[1, 1], a[1, 2], a[2, 1], a[2, 2] );
            b[1, 0] = Det2( a[1, 2], a[1, 0], a[2, 2], a[2, 0] );
            b[2, 0] = Det2( a[1, 0], a[1, 1], a[2, 0], a[2, 1] );
            b[0, 1] = Det2( a[2, 1], a[2, 2], a[0, 1], a[0, 2] );
            b[1, 1] = Det2( a[2, 2], a[2, 0], a[0, 2], a[0, 0] );
            b[2, 1] = Det2( a[2, 0], a[2, 1], a[0, 0], a[0, 1] );
            b[0, 2] = Det2( a[0, 1], a[0, 2], a[1, 1], a[1, 2] );
            b[1, 2] = Det2( a[0, 2], a[0, 0], a[1, 2], a[1, 0] );
            b[2, 2] = Det2( a[0, 0], a[0, 1], a[1, 0], a[1, 1] );

            return b;
        }

        // Calculate matrix for unit square to quad mapping
        private static double[,] MapSquareToQuad( List<IntPoint> quad )
        {
            double[,] sq = new double[3, 3];
            double px, py;

            px = quad[0].X - quad[1].X + quad[2].X - quad[3].X;
            py = quad[0].Y - quad[1].Y + quad[2].Y - quad[3].Y;

            if ( ( px < TOLERANCE ) && ( px > -TOLERANCE ) &&
                 ( py < TOLERANCE ) && ( py > -TOLERANCE ) )
            {
                sq[0, 0] = quad[1].X - quad[0].X;
                sq[0, 1] = quad[2].X - quad[1].X;
                sq[0, 2] = quad[0].X;

                sq[1, 0] = quad[1].Y - quad[0].Y;
                sq[1, 1] = quad[2].Y - quad[1].Y;
                sq[1, 2] = quad[0].Y;

                sq[2, 0] = 0.0;
                sq[2, 1] = 0.0;
                sq[2, 2] = 1.0;
            }
            else
            {
                double dx1, dx2, dy1, dy2, del;

                dx1 = quad[1].X - quad[2].X;
                dx2 = quad[3].X - quad[2].X;
                dy1 = quad[1].Y - quad[2].Y;
                dy2 = quad[3].Y - quad[2].Y;

                del = Det2( dx1, dx2, dy1, dy2 );

                if ( del == 0.0 )
                    return null;

                sq[2, 0] = Det2( px, dx2, py, dy2 ) / del;
                sq[2, 1] = Det2( dx1, px, dy1, py ) / del;
                sq[2, 2] = 1.0;

                sq[0, 0] = quad[1].X - quad[0].X + sq[2, 0] * quad[1].X;
                sq[0, 1] = quad[3].X - quad[0].X + sq[2, 1] * quad[3].X;
                sq[0, 2] = quad[0].X;

                sq[1, 0] = quad[1].Y - quad[0].Y + sq[2, 0] * quad[1].Y;
                sq[1, 1] = quad[3].Y - quad[0].Y + sq[2, 1] * quad[3].Y;
                sq[1, 2] = quad[0].Y;
            }
            return sq;
        }

        // Calculate matrix for general quad to quad mapping
        public static double[,] MapQuadToQuad( List<IntPoint> input, List<IntPoint> output )
        {
            double[,] squareToInpit = MapSquareToQuad( input );
            double[,] squareToOutput = MapSquareToQuad( output );

            if ( squareToOutput == null )
                return null;

            return MultiplyMatrix( squareToOutput, AdjugateMatrix( squareToInpit ) );
        }
    }
}
