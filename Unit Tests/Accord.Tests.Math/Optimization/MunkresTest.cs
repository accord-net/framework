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
    using System;
    using System.Collections.Generic;
    using Accord.Math;
    using Accord.Math.Optimization;
    using NUnit.Framework;
    using Munkres;

    [TestFixture]
    public class MunkresTest
    {

        [Test]
        public void minimize_test()
        {
            #region doc_example
            // This is the same example that is found in Wikipedia:
            // https://en.wikipedia.org/wiki/Hungarian_algorithm

            double[][] costMatrix =
            {
                //                      Clean bathroom, Sweep floors,  Wash windows
                /* Armond   */ new double[] {    2,           3,           3       },
                /* Francine */ new double[] {    3,           2,           3       },
                /* Herbert  */ new double[] {    3,           3,           2       },
            };

            // Create a new Hungarian algorithm
            Munkres m = new Munkres(costMatrix);

            bool success = m.Minimize();    // solve it (should return true)
            double[] solution = m.Solution; // Solution will be 0, 1, 2
            double minimumCost = m.Value;   // should be 6
            #endregion

            Assert.AreEqual(3, m.NumberOfVariables);
            Assert.AreEqual(3, m.NumberOfWorkers);

            Assert.IsTrue(success);
            Assert.AreEqual(6, minimumCost);
            Assert.AreEqual(0, solution[0]);
            Assert.AreEqual(1, solution[1]);
            Assert.AreEqual(2, solution[2]);
        }

        [Test]
        public void minimize_test2()
        {
            double[][] costMatrix = Jagged.Magic(5);

            Munkres m = new Munkres(costMatrix);

            Assert.AreEqual(5, m.NumberOfVariables);
            Assert.AreEqual(5, m.NumberOfWorkers);

            bool success = m.Minimize();
            Assert.IsTrue(success);
            Assert.AreEqual(15, m.Value);
            Assert.AreEqual(2, m.Solution[0]);
            Assert.AreEqual(1, m.Solution[1]);
            Assert.AreEqual(0, m.Solution[2]);
            Assert.AreEqual(4, m.Solution[3]);
            Assert.AreEqual(3, m.Solution[4]);
        }

        [Test]
        public void minimize_test3()
        {
            // http://csclab.murraystate.edu/~bob.pilgrim/445/munkres.html

            double[][] costMatrix =
            {
                new double[] { 1, 2, 3 },
                new double[] { 2, 4, 6 },
                new double[] { 3, 6, 9 },
            };

            Munkres m = new Munkres(costMatrix);

            Assert.AreEqual(3, m.NumberOfVariables);
            Assert.AreEqual(3, m.NumberOfWorkers);

            bool success = m.Minimize();
            Assert.IsTrue(success);
            Assert.AreEqual(10, m.Value);
            Assert.AreEqual(2, m.Solution[0]);
            Assert.AreEqual(1, m.Solution[1]);
            Assert.AreEqual(0, m.Solution[2]);
        }

        [Test]
        public void minimize_test_more_jobs()
        {
            double[][] costMatrix =
            {
                new double[] { 1, 2, 3, 4  },
                new double[] { 2, 4, 6, 8  },
                new double[] { 3, 6, 9, 12 },
            };

            Munkres m = new Munkres(costMatrix);

            Assert.AreEqual(4, m.NumberOfVariables);
            Assert.AreEqual(3, m.NumberOfWorkers);

            bool success = m.Minimize();
            Assert.IsTrue(success);
            Assert.AreEqual(10, m.Value);
            Assert.AreEqual(2, m.Solution[0]);
            Assert.AreEqual(1, m.Solution[1]);
            Assert.AreEqual(0, m.Solution[2]);
            Assert.AreEqual(-1, m.Solution[3]);
        }

        [Test]
        public void minimize_test_more_workers()
        {
            double[][] costMatrix =
            {
                new double[] { 1, 2,  3 },
                new double[] { 2, 4,  6 },
                new double[] { 3, 6,  9 },
                new double[] { 4, 8, 12 },
            };

            Munkres m = new Munkres(costMatrix);

            Assert.AreEqual(3, m.NumberOfVariables);
            Assert.AreEqual(4, m.NumberOfWorkers);

            bool success = m.Minimize();
            Assert.IsTrue(success);
            Assert.AreEqual(10, m.Value);
            Assert.AreEqual(2, m.Solution[0]);
            Assert.AreEqual(1, m.Solution[1]);
            Assert.AreEqual(0, m.Solution[2]);
        }

        [Test]
        public void minimize_rectangular()
        {
            Accord.Math.Random.Generator.Seed = 0;
            double[][] costMatrix = Jagged.Random(10, 7);
            costMatrix.Set(x => x > 0.7, Double.PositiveInfinity);

            Munkres m = new Munkres(costMatrix);

            Assert.AreEqual(7, m.NumberOfVariables);
            Assert.AreEqual(10, m.NumberOfWorkers);

            bool success = m.Minimize();
            Assert.IsTrue(success);
            Assert.AreEqual(1.1474781912506922, m.Value, 1e-10);
            Assert.AreEqual(7, m.Solution[0]);
            Assert.AreEqual(2, m.Solution[1]);
            Assert.AreEqual(3, m.Solution[2]);
            Assert.AreEqual(1, m.Solution[3]);
            Assert.AreEqual(0, m.Solution[4]);
            Assert.AreEqual(8, m.Solution[5]);
            Assert.AreEqual(6, m.Solution[6]);
        }

        [Test]
        public void minimize_index_out_of_range1()
        {
            double[][] costMatrix = new double[][]
            {
                new double[] { 330, 456, 106, 120, 113, 84, 0, 20, 5 },
                new double[] { 246, 360, 62, 72, 69, 44, 8, 44, 9 },
                new double[] { 4.55555555555556, 33.8888888888889, 39.2222222222222, 36.5555555555556, 42.8888888888889, 56.5555555555556, 272.555555555556, 427.222222222222, 272.222222222222 },
                new double[] { 330, 456, 106, 120, 113, 84, 0, 20, 5 },
                new double[] { 246, 360, 62, 72, 69, 44, 8, 44, 9 },
                new double[] { 4.55555555555556, 33.8888888888889, 39.2222222222222, 36.5555555555556, 42.8888888888889, 56.5555555555556, 272.555555555556, 427.222222222222, 272.222222222222 },
                new double[] { 330, 456, 106, 120, 113, 84, 0, 20, 5 },
                new double[] { 246, 360, 62, 72, 69, 44, 8, 44, 9 },
                new double[] { 4.55555555555556, 33.8888888888889, 39.2222222222222, 36.5555555555556, 42.8888888888889, 56.5555555555556, 272.555555555556, 427.222222222222, 272.222222222222 }
            };

            MunkresProgram.Init(costMatrix.ToMatrix());

            var m = new Munkres(costMatrix);
            m.C = costMatrix.Copy();
            int step1 = 1;
            bool done = false;
            while (step1 >= 0)
            {
                step1 = m.RunStep(step1);
                done  = MunkresProgram.RunStep(done);

                Assert.IsTrue(step1 == MunkresProgram.step || (step1 == -1 && MunkresProgram.step == 7));
                Assert.IsTrue(m.C.IsEqual(MunkresProgram.C));
                for (int i = 0; i < m.M.Length; i++)
                    for (int j = 0; j < m.M[i].Length; j++)
                        Assert.AreEqual(m.M[i][j], MunkresProgram.M[i,j]);
            }

            Assert.AreEqual(9, m.NumberOfVariables);
            Assert.AreEqual(9, m.NumberOfWorkers);

            bool success = m.Minimize();
            Assert.IsTrue(success);
            Assert.AreEqual(275, m.Value, 1e-10);
            Assert.AreEqual(8, m.Solution[0]);
            Assert.AreEqual(5, m.Solution[1]);
            Assert.AreEqual(7, m.Solution[2]);
            Assert.AreEqual(2, m.Solution[3]);
            Assert.AreEqual(1, m.Solution[4]);
            Assert.AreEqual(4, m.Solution[5]);
            Assert.AreEqual(6, m.Solution[6]);
            Assert.AreEqual(3, m.Solution[7]);
            Assert.AreEqual(0, m.Solution[8]);
        }


        [Test, Ignore]
        public void minimize_rectangular_inf()
        {
            double inf = Double.PositiveInfinity;

            double[][] costMatrix =
            {
                new double[] { 1.0, inf, inf },
                new double[] { 3.0, inf, inf },
                new double[] { inf, 5.0, 0.5 },
            };

            Munkres m = new Munkres(costMatrix);

            Assert.AreEqual(3, m.NumberOfVariables);
            Assert.AreEqual(3, m.NumberOfWorkers);

            bool success = m.Minimize();
            Assert.IsTrue(success);
            Assert.AreEqual(10, m.Value);
            Assert.AreEqual(2, m.Solution[0]);
            Assert.AreEqual(1, m.Solution[1]);
            Assert.AreEqual(0, m.Solution[2]);
        }
    }
}
