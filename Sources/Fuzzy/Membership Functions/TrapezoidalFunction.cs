// AForge Fuzzy Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace AForge.Fuzzy
{
    using System;
    using AForge;

    /// <summary>
    /// Membership function in the shape of a trapezoid. Can be a half trapzoid if the left or the right side is missing. 
    /// </summary>
    /// 
    /// <remarks><para>Since the <see cref="PiecewiseLinearFunction"/> can represent any piece wise linear
    /// function, it can represent trapezoids too. But as trapezoids are largely used in the creation of
    /// Linguistic Variables, this class simplifies the creation of them. </para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // creating a typical triangular fuzzy set /\
    /// TrapezoidalFunction function1 = new TrapezoidalFunction( 10, 20, 30 );
    /// // creating a right fuzzy set, the rigth side of the set is fuzzy but the left is opened
    /// TrapezoidalFunction function2 = new TrapezoidalFunction( 10, 20, 30, TrapezoidalFunction.EdgeType.Right );
    /// </code>
    /// 
    /// </remarks>
    public class TrapezoidalFunction : PiecewiseLinearFunction
    {
        /// <summary>
        /// Enumeration used to create trapezoidal membership functions with half trapezoids.
        /// </summary>
        /// 
        /// <remarks><para>If the value is Left, the trapezoid has the left edge, but right
        /// is open (/--). If the value is Right, the trapezoid has the right edge, but left
        /// is open (--\).</para></remarks>
        /// 
        public enum EdgeType
        {
            /// <summary>
            /// The fuzzy side of the trapezoid is at the left side.
            /// </summary>
            Left,
            /// <summary>
            /// The fuzzy side of the trapezoid is at the right side.
            /// </summary>
            Right
        };


        /// <summary>
        /// A private constructor used only to reuse code inside of this default constructor.
        /// </summary>
        /// 
        /// <param name="size">Size of points vector to create. This size depends of the shape of the 
        /// trapezoid.</param>
        /// 
        private TrapezoidalFunction( int size )
        {
            points = new Point[size];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrapezoidalFunction"/> class.
        /// 
        /// With four points the shape is known as flat fuzzy number or fuzzy interval (/--\).
        /// </summary>
        /// 
        /// <param name="m1">X value where the degree of membership starts to raise.</param>
        /// <param name="m2">X value where the degree of membership reaches the maximum value.</param>
        /// <param name="m3">X value where the degree of membership starts to fall.</param>
        /// <param name="m4">X value where the degree of membership reaches the minimum value.</param>
        /// <param name="max">The maximum value that the membership will reach, [0, 1].</param>
        /// <param name="min">The minimum value that the membership will reach, [0, 1].</param>
        /// 
        public TrapezoidalFunction( float m1, float m2, float m3, float m4, float max, float min )
            : this( 4 )
        {
            points[0] = new Point( m1, min );
            points[1] = new Point( m2, max );
            points[2] = new Point( m3, max );
            points[3] = new Point( m4, min );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrapezoidalFunction"/> class.
        /// 
        /// With four points the shape is known as flat fuzzy number or fuzzy interval (/--\).
        /// </summary>
        /// 
        /// <param name="m1">X value where the degree of membership starts to raise.</param>
        /// <param name="m2">X value where the degree of membership reaches the maximum value.</param>
        /// <param name="m3">X value where the degree of membership starts to fall.</param>
        /// <param name="m4">X value where the degree of membership reaches the minimum value.</param>
        /// 
        /// <remarks>
        /// <para>Maximum membership value is set to <b>1.0</b> and the minimum is set to <b>0.0</b>.</para>
        /// </remarks>
        /// 
        public TrapezoidalFunction( float m1, float m2, float m3, float m4 )
            : this( m1, m2, m3, m4, 1.0f, 0.0f )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrapezoidalFunction"/> class.
        /// 
        /// With three points the shape is known as triangular fuzzy number or just fuzzy number (/\).
        /// </summary>
        /// 
        /// <param name="m1">X value where the degree of membership starts to raise.</param>
        /// <param name="m2">X value where the degree of membership reaches the maximum value and starts to fall.</param>
        /// <param name="m3">X value where the degree of membership reaches the minimum value.</param>
        /// <param name="max">The maximum value that the membership will reach, [0, 1].</param>
        /// <param name="min">The minimum value that the membership will reach, [0, 1].</param>
        /// 
        public TrapezoidalFunction( float m1, float m2, float m3, float max, float min )
            : this( 3 )
        {
            points[0] = new Point( m1, min );
            points[1] = new Point( m2, max );
            points[2] = new Point( m3, min );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrapezoidalFunction"/> class.
        /// 
        /// With three points the shape is known as triangular fuzzy number or just fuzzy number (/\).
        /// </summary>
        /// 
        /// <param name="m1">X value where the degree of membership starts to raise.</param>
        /// <param name="m2">X value where the degree of membership reaches the maximum value and starts to fall.</param>
        /// <param name="m3">X value where the degree of membership reaches the minimum value.</param>
        /// 
        /// <remarks>
        /// <para>Maximum membership value is set to <b>1.0</b> and the minimum is set to <b>0.0</b>.</para>
        /// </remarks>
        /// 
        public TrapezoidalFunction( float m1, float m2, float m3 )
            : this( m1, m2, m3, 1.0f, 0.0f )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrapezoidalFunction"/> class.
        /// 
        /// With two points and an edge this shape can be a left fuzzy number (/) or a right fuzzy number (\).
        /// </summary>
        /// 
        /// <param name="m1">Edge = Left: X value where the degree of membership starts to raise.
        /// Edge = Right: X value where the function starts, with maximum degree of membership. </param>
        /// <param name="m2">Edge = Left: X value where the degree of membership reaches the maximum.
        /// Edge = Right: X value where the degree of membership reaches minimum value. </param>
        /// <param name="max">The maximum value that the membership will reach, [0, 1].</param>
        /// <param name="min">The minimum value that the membership will reach, [0, 1].</param>
        /// <param name="edge">Trapezoid's <see cref="EdgeType"/>.</param>
        /// 
        public TrapezoidalFunction( float m1, float m2, float max, float min, EdgeType edge )
            : this( 2 )
        {
            if ( edge == EdgeType.Left )
            {
                points[0] = new Point( m1, min );
                points[1] = new Point( m2, max );
            }
            else
            {
                points[0] = new Point( m1, max );
                points[1] = new Point( m2, min );
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrapezoidalFunction"/> class.
        /// 
        /// With three points and an edge this shape can be a left fuzzy number (/--) or a right fuzzy number (--\).
        /// </summary>
        /// 
        /// <param name="m1">Edge = Left: X value where the degree of membership starts to raise.
        /// Edge = Right: X value where the function starts, with maximum degree of membership. </param>
        /// <param name="m2">Edge = Left: X value where the degree of membership reaches the maximum.
        /// Edge = Right: X value where the degree of membership reaches minimum value. </param>
        /// <param name="edge">Trapezoid's <see cref="EdgeType"/>.</param>
        /// 
        /// <remarks>
        /// <para>Maximum membership value is set to <b>1.0</b> and the minimum is set to <b>0.0</b>.</para>
        /// </remarks>
        /// 
        public TrapezoidalFunction( float m1, float m2, EdgeType edge )
            : this( m1, m2, 1.0f, 0.0f, edge )
        {
        }
    }
}

