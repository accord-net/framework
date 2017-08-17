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
    using Accord.Statistics.Models.Fields.Learning;
    using NUnit.Framework;
    using Accord.Statistics.Models.Fields;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Models.Markov.Topology;
    using Accord.Statistics.Models.Fields.Functions;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Math;
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Distributions.Univariate;

    [TestFixture]
    public class QuasiNewtonLearningTest
    {


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

        [Test]
        public void RunTest()
        {
            Accord.Math.Random.Generator.Seed = 0;

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


            var function = new MarkovDiscreteFunction(nstates, symbols, new NormalDistribution());
            var model = new ConditionalRandomField<int>(nstates, function);


            for (int i = 0; i < sequences.Length; i++)
            {
                double p;
                int[] s = sequences[i];
                int[] r = model.Compute(s, out p);
                Assert.IsFalse(s.IsEqual(r));
            }

            var target = new QuasiNewtonLearning<int>(model);

            target.ParallelOptions.MaxDegreeOfParallelism = 1;

            int[][] labels = sequences;
            int[][] observations = sequences;

            double ll0 = model.LogLikelihood(observations, labels);

            double actual = target.Run(observations, labels);

            double ll1 = model.LogLikelihood(observations, labels);

            Assert.IsTrue(ll1 > ll0);


            Assert.AreEqual(-0.0010766857305242183, actual, 1e-6);

            for (int i = 0; i < sequences.Length; i++)
            {
                double p;
                int[] s = sequences[i];
                int[] r = model.Compute(s, out p);
                Assert.IsTrue(s.IsEqual(r));
            }

        }


        [Test]
        public void learn_test()
        {
            #region doc_learn
            Accord.Math.Random.Generator.Seed = 0;

            int[][] input =
            {
                new int[] { 0,1,1,1,0,0 },
                new int[] { 0,1,1,0 },
                new int[] { 0,1,1,0,0,0 },
                new int[] { 0,1,1,1,1,0 },
                new int[] { 0,1,1,1,0,0,0,0 },
                new int[] { 0,1,1,1,0,0 },
                new int[] { 0,1,1,0 },
                new int[] { 0,1,1,1,0 },
            };

            int[][] output =
            {
                new int[] { 0,0,1,1,1,2 },
                new int[] { 0,0,1,2 },
                new int[] { 0,0,1,1,1,2 },
                new int[] { 0,0,1,1,1,2 },
                new int[] { 0,0,1,1,1,1,2,2 },
                new int[] { 0,0,1,1,1,2 },
                new int[] { 0,0,1,2 },
                new int[] { 0,1,1,1,2 },
            };

            // Create a new L-BFGS learning algorithm
            var teacher = new QuasiNewtonLearning<int>()
            {
                Function = new MarkovDiscreteFunction(states: 3, symbols: 2, initialization: new NormalDistribution())
            };

            // Learn the Conditional Random Field model
            ConditionalRandomField<int> crf = teacher.Learn(input, output);

            // Use the model to classify the samples
            int[][] prediction = crf.Decide(input);

            var cm = new GeneralConfusionMatrix(predicted: prediction.Reshape(), expected: output.Reshape());
            double acc = cm.Accuracy;
            #endregion

            Assert.IsTrue(acc > 0.7);
        }

    }
}
