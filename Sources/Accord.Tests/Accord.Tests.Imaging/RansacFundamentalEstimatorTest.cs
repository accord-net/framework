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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using AForge;
using Accord.Math;
using System;
using Accord.Imaging;

namespace Accord.Tests.Imaging
{


    [TestClass()]
    public class RansacFundamentalEstimatorTest
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
        public void EstimateTest()
        {

            Point[] points1 = 
            {
                new Point(86, 3),
                new Point(262, 7),
                new Point(72, 12),
                new Point(233, 14),
                new Point(222, 16),
                new Point(242, 19),
                new Point(174, 21),
                new Point(199, 22),
                new Point(210, 23),
                new Point(245, 27),
                new Point(223, 28),
                new Point(171, 29),
                new Point(213, 32),
                new Point(206, 34),
                new Point(158, 36),
                new Point(215, 36),
                new Point(194, 40),
                new Point(155, 43),
                new Point(390, 145),
                new Point(325, 151),
                new Point(430, 165),
                new Point(569, 166),
                new Point(548, 171),
                new Point(486, 172),
                new Point(585, 174),
                new Point(513, 175),
                new Point(581, 178)
            };


            //Points 2
            Point[] points2 = 
            {
                new Point(94, 3),
                new Point(129, 10),
                new Point(135, 6),
                new Point(100, 16),
                new Point(88, 18),
                new Point(109, 22),
                new Point(35, 23),
                new Point(63, 24),
                new Point(75, 25),
                new Point(112, 30),
                new Point(89, 31),
                new Point(32, 31),
                new Point(78, 35),
                new Point(70, 37),
                new Point(19, 38),
                new Point(80, 39),
                new Point(58, 43),
                new Point(15, 46),
                new Point(259, 151),
                new Point(194, 158),
                new Point(299, 171),
                new Point(433, 171),
                new Point(414, 176),
                new Point(354, 177),
                new Point(449, 178),
                new Point(380, 180),
                new Point(445, 183)
            };


            double[,] expected = 
            {
		        {  7.07032436087535e-07,  4.33250001914367e-05,  0.000202442793261960 },
                { -4.24949673892611e-05,  7.57829117692283e-07, -0.0167251164924736   },
                {  6.04519481294552e-05,  0.0112588312334709,   -0.00408283512108965  },
            };


            // Set a fixed seed to transform RANSAC into a deterministic algorithm
            Accord.Math.Tools.SetupGenerator(0);

            RansacFundamentalEstimator ransac = new RansacFundamentalEstimator(0.001, 0.99);
            float[,] actual = ransac.Estimate(points1, points2);


            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    Assert.AreEqual(expected[i, j], actual[i, j], 1e-3);

        }
    }
}
