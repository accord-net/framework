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
    /// NOT operator, used to calculate the complement of a fuzzy set. 
    /// </summary>
    /// 
    /// <remarks><para>The NOT operator definition is (1 - m) for all the values of membership m of the fuzzy set.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // creating a fuzzy sets to represent Cool (Temperature)
    /// TrapezoidalFunction function1 = new TrapezoidalFunction( 13, 18, 23, 28 );
    /// FuzzySet fsCool = new FuzzySet( "Cool", function1 );
    /// 
    /// // getting membership
    /// float m1 = fsCool.GetMembership( 15 );
    /// 
    /// // computing the membership of "NOT Cool"
    /// NotOperator NOT = new NotOperator( );
    /// float result = NOT.Evaluate( m1 );
    ///              
    /// // show result
    /// Console.WriteLine( result );
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="IUnaryOperator"/>
    /// 
    public class NotOperator : IUnaryOperator
    {
        /// <summary>
        /// Calculates the numerical result of the NOT operation applied to
        /// a fuzzy membership value.
        /// </summary>
        /// 
        /// <param name="membership">A fuzzy membership value, [0..1].</param>
        /// 
        /// <returns>The numerical result of the unary operation NOT applied to <paramref name="membership"/>.</returns>
        /// 
        public float Evaluate( float membership )
        {
            return ( 1 - membership );
        }
    }
}

