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

namespace Accord.Tests.Math
{
    using Accord.Math.Decompositions;
    using NUnit.Framework;
    using Accord.Math;

    [TestFixture]
    public class NonnegativeMatrixFactorizationTest
    {

        [Test]
        public void NonNegativeMatrixFactorizationConstructorTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[,] X =
            {
                { 1,     0,     5 },
                { 1,     2,     1 },
                { 0,     6,     1 },
                { 2,     6,     5 },
                { 2,     1,     1 },
                { 5,     1,     1 }
            };


            var nmf = new NonnegativeMatrixFactorization(X, 3);

            var H = nmf.RightNonnegativeFactors;
            var W = nmf.LeftNonnegativeFactors;

            var R = Matrix.Multiply(W, H).Transpose();
            Assert.IsTrue(R.IsEqual(X, 0.05));
        }

        [Test]
        public void NonNegativeMatrixFactorizationConstructorTest2()
        {
            double[] data = 
            {
                0.814723686, 0.157613082, 0.655740699, 0.706046088, 0.43874436, 0.276025077, 0.751267059, 0.840717256, 0.351659507, 0.07585429,
                0.905791937, 0.970592782, 0.035711679, 0.031832846, 0.381558457, 0.679702677, 0.255095115, 0.254282179, 0.830828628, 0.053950119,
                0.126986816, 0.957166948, 0.849129306, 0.276922985, 0.765516788, 0.655098004, 0.505957052, 0.814284826, 0.585264091, 0.530797553,
                0.913375856, 0.485375649, 0.933993248, 0.046171391, 0.795199901, 0.162611735, 0.699076723, 0.243524969, 0.549723608, 0.77916723,
                0.632359246, 0.800280469, 0.678735155, 0.097131781, 0.186872605, 0.118997682, 0.890903253, 0.929263623, 0.917193664, 0.934010684,
                0.097540405, 0.141886339, 0.757740131, 0.823457828, 0.489764396, 0.498364052, 0.959291425, 0.349983766, 0.285839019, 0.129906208,
                0.278498219, 0.421761283, 0.743132468, 0.694828623, 0.445586201, 0.959743959, 0.54721553, 0.19659525, 0.757200229, 0.568823661,
                0.546881519, 0.915735525, 0.39222702, 0.31709948, 0.64631301, 0.340385727, 0.138624443, 0.251083858, 0.753729094, 0.469390641,
                0.957506835, 0.79220733, 0.65547789, 0.950222049, 0.709364831, 0.585267751, 0.149294006, 0.616044676, 0.380445847, 0.01190207,
                0.964888535, 0.959492426, 0.171186688, 0.034446081, 0.754686682, 0.223811939, 0.257508254, 0.473288849, 0.567821641, 0.337122644
            };

            double[,] input = data.Reshape(10, 10);

            var nmf = new NonnegativeMatrixFactorization(input, 2);

            var H = nmf.RightNonnegativeFactors;
            var W = nmf.LeftNonnegativeFactors;

            Assert.IsFalse(H.Has(0));
            Assert.IsFalse(W.Has(0));
        }

    }
}
