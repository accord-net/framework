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
    /// Membership function used in fuzzy singletons: fuzzy sets that have just one point with membership value 1. 
    /// </summary>
    /// 
    /// <remarks><para>Sometimes it is needed to represent crisp (classical) number in the fuzzy domain. Several approaches 
    /// can be used, like adding some uncertain (fuzziness) in the original number (the number one, for instance, can be seen as a <see cref="TrapezoidalFunction"/>
    /// with -0.5, 1.0 and 0.5 parameters). Another approach is to declare fuzzy singletons: fuzzy sets with only one point returning a none zero membership.</para>
    /// 
    /// <para>While trapezoidal and half trapezoidal are classic functions used in fuzzy functions, this class supports any function
    /// or approximation that can be represented as a sequence of lines.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // creating the instance
    /// SingletonFunction membershipFunction = new SingletonFunction( 10 );
    /// // getting membership for several points
    /// for ( int i = 0; i &lt; 20; i++ )
    ///     Console.WriteLine( membershipFunction.GetMembership( i ) );
    /// </code>
    /// </remarks>
    /// 
    public class SingletonFunction : IMembershipFunction
    {
        /// <summary>
        /// The unique point where the membership value is 1.
        /// </summary>
        protected float support;

        /// <summary>
        /// The leftmost x value of the membership function, the same value of the support.
        /// </summary>
        /// 
        public float LeftLimit
        {
            get
            {
                return support;
            }
        }

        /// <summary>
        /// The rightmost x value of the membership function, the same value of the support.
        /// </summary>
        /// 
        public float RightLimit
        {
            get
            {
                return support;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonFunction"/> class. 
        /// </summary>
        /// 
        /// <param name="support">Support is the only value of x where the membership function is 1.</param>
        /// 
        public SingletonFunction( float support )
        {
            this.support = support;
        }

        /// <summary>
        /// Calculate membership of a given value to the singleton function.
        /// </summary>
        /// 
        /// <param name="x">Value which membership will to be calculated.</param>
        /// 
        /// <returns>Degree of membership {0,1} since singletons do not admit memberships different from 0 and 1. </returns>
        /// 
        public float GetMembership( float x )
        {
            // if x is the support, returns 1, otherwise, returns 0
            return ( support == x ) ? 1 : 0;
        }
    }
}
