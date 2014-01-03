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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Models.Fields.Learning;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics.Models.Fields;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Models.Markov.Topology;
    using Accord.Statistics.Models.Fields.Functions;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Math;

    [TestClass()]
    public class QuasiNewtonLearningTest
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


        private static HiddenMarkovModel trainHMM()
        {
            int states = 3;
            int symbols = 3;

            int[][] sequences = new int[][] 
            {
                new int[] { 0, 1, 1, 1, 2 },
                new int[] { 0, 1, 1, 1, 2, 2, 2 },
                new int[] { 0, 0, 1, 1, 2, 2 },
                new int[] { 0, 1, 1, 1, 2, 2, 2 },
                new int[] { 0, 1, 1, 1, 2, 1 },
                new int[] { 0, 1, 1, 2, 2 },
                new int[] { 0, 0, 1, 1, 1, 2, 1 },
                new int[] { 0, 0, 0, 1, 1, 1, 2, 1 },
                new int[] { 0, 1, 1, 2, 2, 2 },
            };

            HiddenMarkovModel hmm = new HiddenMarkovModel(new Forward(states), symbols);

            var teacher = new BaumWelchLearning(hmm) { Iterations = 100, Tolerance = 0 };

            double ll = teacher.Run(sequences);

            return hmm;
        }

        [TestMethod()]
        public void RunTest()
        {
            int nstates = 3;
            int symbols = 3;

            int[][] sequences = new int[][] 
            {
                new int[] { 0, 1, 1, 1, 2 },
                new int[] { 0, 1, 1, 1, 2, 2, 2 },
                new int[] { 0, 0, 1, 1, 2, 2 },
                new int[] { 0, 1, 1, 1, 2, 2, 2 },
                new int[] { 0, 1, 1, 1, 2, 2 },
                new int[] { 0, 1, 1, 2, 2 },
                new int[] { 0, 0, 1, 1, 1, 2, 2 },
                new int[] { 0, 0, 0, 1, 1, 1, 2, 2 },
                new int[] { 0, 1, 1, 2, 2, 2 },
            };


            var function = new MarkovDiscreteFunction(nstates, symbols);
            var model = new ConditionalRandomField<int>(nstates, function);


            for (int i = 0; i < sequences.Length; i++)
            {
                double p;
                int[] s = sequences[i];
                int[] r = model.Compute(s, out p);
                Assert.IsFalse(s.IsEqual(r));
            }

            var target = new QuasiNewtonLearning<int>(model); 

            int[][] labels = sequences;
            int[][] observations = sequences;

            double ll0 = model.LogLikelihood(observations, labels);

            double actual = target.Run(observations, labels);

            double ll1 = model.LogLikelihood(observations, labels);

            Assert.IsTrue(ll1 > ll0);


            Assert.AreEqual(-0.01205815673780819, actual, 1e-10);

            for (int i = 0; i < sequences.Length; i++)
            {
                double p;
                int[] s = sequences[i];
                int[] r = model.Compute(s, out p);
                Assert.IsTrue(s.IsEqual(r));
            }
            
        }

    }
}
