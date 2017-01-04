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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Analysis;
    using NUnit.Framework;
    using System;
    using Accord.Math;

    [TestFixture]
    public class ProcrustesAnalysisTest
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



        [Test]
        public void ProcrustesAnalysisConstructorTest()
        {
            // Define a square
            double[,] square = { { 100, 100 },
                                 { 300, 100 },
                                 { 300, 300 },
                                 { 100, 300 }
                               };
            // Define a diamond with different orientation and scale
            double[,] diamond = { { 170, 120 },
                                  { 220, 170 },
                                  { 270, 120 },
                                  { 220, 70 }
                                };

            // Create the Procrustes analysis object
            ProcrustesAnalysis pa = new ProcrustesAnalysis(square, diamond);

            // Compute the analysis on the square and the diamond
            pa.Compute();

            // Assert that the diamond is a square
            Assert.AreEqual(0.0, pa.ProcrustesDistances[0, 1], 1E-11);

            // Transform the diamond to a square
            double[,] diamond_to_a_square = pa.ProcrustedDatasets[1].Transform(pa.ProcrustedDatasets[0]);

            // Check that the diamond matches (quite) perfectly the square
            for (int i = 0; i < diamond_to_a_square.GetLength(0); i++)
            {
                for (int j = 0; j < diamond_to_a_square.GetLength(1); j++)
                {
                    Assert.AreEqual(diamond_to_a_square[i, j], square[i, j], 1E-11);
                }
            }
        }

    }
}
