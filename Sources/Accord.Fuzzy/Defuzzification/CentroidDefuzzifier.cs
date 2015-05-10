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
    using System.Collections.Generic;

    /// <summary>
    /// This class implements the centroid defuzzification method.
    /// </summary>
    /// 
    /// <remarks><para>In many applications, a Fuzzy Inference System is used to perform linguistic
    /// computation, but at the end of the inference process, a numerical value is needed. It does
    /// not mean that the system needs precision, but simply that a numerical value is required,
    /// most of the times because it will be used to control another system that needs the number.
    /// To obtain this numer, a defuzzification method is performed.</para>
    /// 
    /// <para>This class implements the centroid defuzzification method. The output of a Fuzzy
    /// Inference System is a set of rules (see <see cref="Rule"/>) with firing strength greater
    /// than zero. Those firing strength apply a constraint to the consequent fuzzy sets
    /// (see <see cref="FuzzySet"/>) of the rules. Putting all those fuzzy sets togheter results
    /// in a a shape that is the linguistic output meaning. 
    /// </para>
    /// 
    /// <para>The centroid method calculates the center of the area of this shape to obtain the
    /// numerical representation of the output. It uses a numerical approximation, so a number
    /// of intervals must be choosen. As the number of intervals grow, the precision of the
    /// numerical ouput grows. 
    /// </para>
    /// 
    /// <para>For a sample usage of the <see cref="CentroidDefuzzifier"/> see <see cref="InferenceSystem"/>
    /// class.</para>
    /// </remarks>
    /// 
    public class CentroidDefuzzifier : IDefuzzifier
    {
        // number of intervals to use in numerical approximation
        private int intervals;

        /// <summary>
        /// Initializes a new instance of the <see cref="CentroidDefuzzifier"/> class.
        /// </summary>
        /// 
        /// <param name="intervals">Number of segments that the speech universe will be splited
        /// to perform the numerical approximation of the center of area.</param>
        /// 
        public CentroidDefuzzifier( int intervals )
        {
            this.intervals = intervals;
        }

        /// <summary>
        /// Centroid method to obtain the numerical representation of a fuzzy output. The numerical
        /// value will be the center of the shape formed by the several fuzzy labels with their
        /// constraints.
        /// </summary>
        /// 
        /// <param name="fuzzyOutput">A <see cref="FuzzyOutput"/> containing the output of several
        /// rules of a Fuzzy Inference System.</param>
        /// <param name="normOperator">A <see cref="INorm"/> operator to be used when constraining
        /// the label to the firing strength.</param>
        /// 
        /// <returns>The numerical representation of the fuzzy output.</returns>
        /// 
        /// <exception cref="Exception">The numerical output is unavaliable. All memberships are zero.</exception>
        /// 
        public float Defuzzify( FuzzyOutput fuzzyOutput, INorm normOperator )
        {
            // results and accumulators
            float weightSum = 0, membershipSum = 0;

            // speech universe
            float start = fuzzyOutput.OutputVariable.Start;
            float end = fuzzyOutput.OutputVariable.End;

            // increment
            float increment = ( end - start ) / this.intervals;

            // running through the speech universe and evaluating the labels at each point
            for ( float x = start; x < end; x += increment )
            {
                // we must evaluate x membership to each one of the output labels
                foreach ( FuzzyOutput.OutputConstraint oc in fuzzyOutput.OutputList )
                {
                    // getting the membership for X and constraining it with the firing strength
                    float membership = fuzzyOutput.OutputVariable.GetLabelMembership( oc.Label, x );
                    float constrainedMembership = normOperator.Evaluate( membership, oc.FiringStrength );

                    weightSum += x * constrainedMembership;
                    membershipSum += constrainedMembership;
                }
            }

            // if no membership was found, then the membershipSum is zero and the numerical output is unknown.
            if ( membershipSum == 0 )
                throw new Exception( "The numerical output in unavaliable. All memberships are zero." );

            return weightSum / membershipSum;
        }
    }
}
