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


namespace Accord.Tests.Statistics.Models.Fields
{
    using Accord.Statistics.Models.Fields;
    using NUnit.Framework;
    using Accord.Statistics.Models.Fields.Functions;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Math;
    using Accord.Statistics.Models.Markov.Topology;


    [TestFixture]
    public class ConditionalRandomFieldTest
    {

        [Test]
        public void ConditionalRandomFieldConstructorTest()
        {
            HiddenMarkovModel hmm = DiscreteHiddenMarkovModelFunctionTest.CreateModel1();

            int states = 2;
            var function = new MarkovDiscreteFunction(hmm);
            var target = new ConditionalRandomField<int>(states, function);


            Assert.AreEqual(function, target.Function);
            Assert.AreEqual(2, target.States);
        }

        [Test]
        public void ComputeTest()
        {

            HiddenMarkovModel hmm = DiscreteHiddenMarkovModelFunctionTest.CreateModel2();

            int states = hmm.States;


            var function = new MarkovDiscreteFunction(hmm);
            var target = new ConditionalRandomField<int>(states, function);
            double p1, p2;

            int[] observations, expected, actual;

            observations = new int[] { 0, 0, 1, 1, 1, 2 };
            expected = hmm.Decode(observations, out p1);
            actual = target.Compute(observations, out p2);

            Assert.IsTrue(expected.IsEqual(actual));
            Assert.AreEqual(p1, p2, 1e-6);


            observations = new int[] { 0, 1, 2, 2, 2 };
            expected = hmm.Decode(observations, out p1);
            actual = target.Compute(observations, out p2);

            Assert.IsTrue(expected.IsEqual(actual));
            Assert.AreEqual(p1, p2, 1e-6);
        }

        [Test]
        public void LikelihoodTest()
        {
            var hmm = DiscreteHiddenMarkovModelFunctionTest.CreateModel2();
            
            int states = hmm.States;
            int symbols = hmm.Symbols;

            var hcrf = new ConditionalRandomField<int>(states,
                new MarkovDiscreteFunction(hmm));

            var hmm0 = new HiddenMarkovModel(states, symbols);
            var hcrf0 = new ConditionalRandomField<int>(states, 
                new MarkovDiscreteFunction(hmm0));


            int[] observations = new int[] { 0, 0, 1, 1, 1, 2 };
            double la = hcrf.LogLikelihood(observations, observations);
            double lb = hcrf0.LogLikelihood(observations, observations);
            Assert.IsTrue(la > lb);

            double lc = hmm.Evaluate(observations, observations);
            double ld = hmm0.Evaluate(observations, observations);
            Assert.IsTrue(lc > ld);

            double za = hcrf.LogPartition(observations);
            double zb = hcrf0.LogPartition(observations);

            la += za;
            lb += zb;

            Assert.AreEqual(la, lc, 1e-6);
            Assert.AreEqual(lb, ld, 1e-6);
        }
      
    }
}
