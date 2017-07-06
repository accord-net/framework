// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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

#if !DEBUG

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;
    using System;
    using Accord.Math;
    using Accord.Math.Differentiation;


    [TestFixture]
    public class ContinuousInheritanceTest
    {



        class MyDistribution : UnivariateDiscreteDistribution
        {

            public override double Mean
            {
                get { return 0; }
            }

            public override double Variance
            {
                get { return 1; }
            }


            public override double Entropy
            {
                get { return System.Math.Log(2); }
            }

            public override IntRange Support
            {
                get { return new IntRange(-1, +1); }
            }

            public override double DistributionFunction(int k)
            {
                if (k < -1)
                    return 0;
                if (k >= 1)
                    return 1;
                return 0.5;
            }

            public override double ProbabilityMassFunction(int k)
            {
                if (k == -1)
                    return 0.5;
                if (k == +1)
                    return 0.5;
                return 0.0;
            }

            public override string ToString(string format, IFormatProvider formatProvider)
            {
                return "MyDistribution(x)";
            }

            public override object Clone()
            {
                return new MyDistribution();
            }
        }
    }

}

#endif