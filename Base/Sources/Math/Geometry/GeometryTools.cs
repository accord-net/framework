// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace AForge.Math.Geometry
{
    using System;

    /// <summary>
    /// Collection of some gemetry tool methods.
    /// </summary>
    /// 
    public static class GeometryTools
    {
        /// <summary>
        /// Calculate angle between to vectors measured in [0, 180] degrees range.
        /// </summary>
        /// 
        /// <param name="startPoint">Starting point of both vectors.</param>
        /// <param name="vector1end">Ending point of the first vector.</param>
        /// <param name="vector2end">Ending point of the second vector.</param>
        /// 
        /// <returns>Returns angle between specified vectors measured in degrees.</returns>
        /// 
        public static float GetAngleBetweenVectors( Point startPoint, Point vector1end, Point vector2end )
        {
            float x1 = vector1end.X - startPoint.X;
            float y1 = vector1end.Y - startPoint.Y;

            float x2 = vector2end.X - startPoint.X;
            float y2 = vector2end.Y - startPoint.Y;

            return (float) ( Math.Acos( ( x1 * x2 + y1 * y2 ) / ( Math.Sqrt( x1 * x1 + y1 * y1 ) * Math.Sqrt( x2 * x2 + y2 * y2 ) ) ) * 180.0 / Math.PI );
        }

        /// <summary>
        /// Calculate minimum angle between two lines measured in [0, 90] degrees range.
        /// </summary>
        /// 
        /// <param name="a1">A point on the first line.</param>
        /// <param name="a2">Another point on the first line.</param>
        /// <param name="b1">A point on the second line.</param>
        /// <param name="b2">Another point on the second line.</param>
        /// 
        /// <returns>Returns minimum angle between two lines.</returns>
        /// 
        /// <remarks><para><note>It is preferred to use <see cref="Line.GetAngleBetweenLines"/> if it is required to calculate angle
        /// multiple times for one of the lines.</note></para></remarks>
        /// 
        /// <exception cref="ArgumentException"><paramref name="a1"/> and <paramref name="a2"/> are the same,
        /// -OR- <paramref name="b1"/> and <paramref name="b2"/> are the same.</exception>
        /// 
        public static float GetAngleBetweenLines( Point a1, Point a2, Point b1, Point b2 )
        {
            Line line1 = Line.FromPoints( a1, a2 );
            return line1.GetAngleBetweenLines( Line.FromPoints( b1, b2 ) );
        }
    }
}
