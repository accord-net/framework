// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Statistics.Distributions.Multivariate
{
    using System;
    using Accord.Math;
    using Accord.Statistics.Distributions.Fitting;

    /// <summary>
    ///   Joint distribution assuming independence between vector components.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability and statistics, given at least two random variables <c>X</c>, 
    ///   <c>Y</c>, ..., that are defined on a probability space, the joint probability 
    ///   distribution for <c>X</c>, <c>Y</c>, ... is a probability distribution that 
    ///   gives the probability that each of X, Y, ... falls in any particular range or
    ///   discrete set of values specified for that variable. In the case of only two 
    ///   random variables, this is called a bivariate distribution, but the concept 
    ///   generalizes to any number of random variables, giving a multivariate distribution.
    /// </para>
    /// 
    /// <para>
    ///   This class is also available in a generic version, allowing for any
    ///   choice of component distribution (<see cref="Independent{TDistribution}"/>.
    /// </para>
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Joint_probability_distribution">
    ///       Wikipedia, The Free Encyclopedia. Beta distribution. 
    ///       Available from: http://en.wikipedia.org/wiki/Joint_probability_distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <para>
    ///   The following example shows how to declare and initialize an Independent Joint
    ///   Gaussian Distribution using known means and variances for each component.</para>
    ///   
    /// <code>
    ///   // Declare two normal distributions
    ///   NormalDistribution pa = new NormalDistribution(4.2, 1); // p(a)
    ///   NormalDistribution pb = new NormalDistribution(7.0, 2); // p(b)
    ///  
    ///   // Now, create a joint distribution combining these two:
    ///   var joint = new Independent(pa, pb);
    ///   
    ///   // This distribution assumes the distributions of the two components are independent,
    ///   // i.e. if we have 2D input vectors on the form {a, b}, then p({a,b}) = p(a) * p(b). 
    ///   
    ///   // Lets check a simple example. Consider a 2D input vector x = { 4.2, 7.0 } as
    ///   //
    ///   double[] x = new double[] { 4.2, 7.0 };
    ///   
    ///   // Those two should be completely equivalent:
    ///   double p1 = joint.ProbabilityDensityFunction(x);
    ///   double p2 = pa.ProbabilityDensityFunction(x[0]) * pb.ProbabilityDensityFunction(x[1]);
    ///   
    ///   bool equal = p1 == p2; // at this point, equal should be true.
    /// </code>
    /// 
    /// <seealso cref="Independent{TDistribution}"/>
    /// 
    [Serializable]
    public class Independent : Independent<IUnivariateDistribution>
    {

        /// <summary>
        ///   Initializes a new instance of the <see cref="Independent&lt;TDistribution&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="components">The components.</param>
        /// 
        public Independent(params IUnivariateDistribution[] components)
            : base(components) { }


        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public override object Clone()
        {
            var clone = new IUnivariateDistribution[Components.Length];
            for (int i = 0; i < clone.Length; i++)
                clone[i] = (IUnivariateDistribution)Components[i].Clone();

            return new Independent(clone);
        }

    }
}
