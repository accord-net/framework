// Accord Unit Tests
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

namespace Accord.Tests.Math
{
    using Accord.Math.Environments;
    using Accord.Math;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass()]
    public class OctaveEnvironmentTest : OctaveEnvironment
    {


        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }



        [TestMethod()]
        public void eyeTest()
        {
            var I = eye(3);

            var A = I * 2;

            Console.WriteLine(A);
            //
            //        2 0 0
            //    A = 0 2 0
            //        0 0 2

            var B = ones(3, 6);

            Console.WriteLine(B);
            //
            //        1 1 1 1 1 1
            //    B = 1 1 1 1 1 1
            //        1 1 1 1 1 1

            var C = new double[,]
            {
                { 2, 2, 2, 2, 2, 2 },
                { 2, 0, 0, 0, 0, 2 },
                { 2, 2, 2, 2, 2, 2 },
            };

            var D = A * B - C;

            Console.WriteLine(D);
            //
            //        0 0 0 0 0 0
            //    C = 0 2 2 2 2 0
            //        0 0 0 0 0 0

            double[,] expectedD = 
            {
                { 0, 0, 0, 0, 0, 0 },
                { 0, 2, 2, 2, 2, 0 },
                { 0, 0, 0, 0, 0, 0 },
            };

            Assert.IsTrue(expectedD.IsEqual(D));
        }

        [TestMethod()]
        public void svdTest()
        {
            // Declare local matrices
            mat u = _, s = _, v = _;

            // Compute a new mat
            mat M = magic(3) * 5;

            // Compute the SVD
            ret[u, s, v] = svd(M);

            string str = u;

            /*
                            0.577350269189626 -0.707106781186548     0.408248290463863 
                       U =  0.577350269189626 -1.48007149071427E-16 -0.816496580927726 
                            0.577350269189626  0.707106781186548     0.408248290463863
            */
            double[,] expectedU = 
            {
                { 0.577350269189626, -0.707106781186548,     0.408248290463863 },
                { 0.577350269189626, -1.48007149071427E-16, -0.816496580927726 }, 
                { 0.577350269189626,  0.707106781186548,     0.408248290463863 },
            };

            double[,] expectedS =
            {
                { 74.999999999999972, 0, 0 },
                { 0, 34.641016151377556, 0 },
                { 0, 0, 17.320508075688775 },
            };

            double[,] expectedV = 
            {
                { 0.57735026918962573, -0.4082482904638628, 0.70710678118654779 },
                { 0.57735026918962562, 0.81649658092772615, -0.000000000000000061130671974381729 },
                { 0.57735026918962584, -0.40824829046386324, -0.70710678118654757 },
            };

            Assert.IsTrue(expectedU.IsEqual(u, 1e-10));
            Assert.IsTrue(expectedS.IsEqual(s, 1e-10));
            Assert.IsTrue(expectedV.IsEqual(v, 1e-10));
        }
    }
}
