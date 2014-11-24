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

    /// <summary>
    /// The class represents a fuzzy set.
    /// </summary>
    /// 
    /// <remarks><para>The fuzzy sets are the base for all fuzzy applications. In a classical set, the membership of 
    /// a given value to the set can always be defined as true (1) or false (0). In fuzzy sets, this membership can be 
    /// a value in the range [0..1], representing the imprecision existent in many real world applications.</para>
    /// 
    /// <para>Let us consider, for example, fuzzy sets representing some temperature. In a given application, there is the 
    /// need to represent a cool and warm temperature. Like in real life, the precise point when the temperature changes from 
    /// cool to warm is not easy to find, and does not makes sense. If we consider the cool around 20 degrees and warm around 
    /// 30 degrees, it is not simple to find a break point. If we take the mean, we can consider values greater than or equal 
    /// 25 to be warm. But we can still consider 25 a bit cool. And a bit warm at the same time. This is where fuzzy sets can 
    /// help.</para>
    /// 
    /// <para>Fuzzy sets are often used to compose Linguistic Variables, used in Fuzzy Inference Systems.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // creating 2 fuzzy sets to represent Cool and Warm
    /// TrapezoidalFunction function1 = new TrapezoidalFunction( 13, 18, 23, 28 );
    /// FuzzySet fsCool = new FuzzySet( "Cool", function1 );
    /// TrapezoidalFunction function2 = new TrapezoidalFunction( 23, 28, 33, 38 );
    /// FuzzySet fsWarm = new FuzzySet( "Warm", function2 );
    /// 
    /// // show membership to the Cool set for some values 
    /// Console.WriteLine( "COOL" );
    /// for ( int i = 13; i &lt;= 28; i++ )
    ///     Console.WriteLine( fsCool.GetMembership( i ) );
    /// 
    /// // show membership to the Warm set for some values 
    /// Console.WriteLine( "WARM" );
    /// for ( int i = 23; i &lt;= 38; i++ )
    ///     Console.WriteLine( fsWarm.GetMembership( i ) );
    /// </code>    
    /// </remarks>
    /// 
    public class FuzzySet
    {
        // name of the fuzzy set
        private string name;
        // membership functions that defines the shape of the fuzzy set
        private IMembershipFunction function;

        /// <summary>
        /// Name of the fuzzy set.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// The leftmost x value of the fuzzy set's membership function.
        /// </summary>
        /// 
        public float LeftLimit
        {
            get
            {
                return function.LeftLimit;
            }
        }

        /// <summary>
        /// The rightmost x value of the fuzzy set's membership function.
        /// </summary>
        /// 
        public float RightLimit
        {
            get
            {
                return function.RightLimit;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FuzzySet"/> class.
        /// </summary>
        /// 
        /// <param name="name">Name of the fuzzy set.</param>
        /// <param name="function">Membership function that will define the shape of the fuzzy set. </param>
        /// 
        public FuzzySet( string name, IMembershipFunction function )
        {
            this.name     = name;
            this.function = function;
        }

        /// <summary>
        /// Calculate membership of a given value to the fuzzy set.
        /// </summary>
        /// 
        /// <param name="x">Value which membership needs to be calculated.</param>
        /// 
        /// <returns>Degree of membership [0..1] of the value to the fuzzy set.</returns>
        /// 
        public float GetMembership( float x )
        {
            return function.GetMembership( x );
        }
    }
}
