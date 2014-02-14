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


namespace Accord.Tests.Statistics.Models.Fields
{
    using Accord.Statistics.Models.Fields;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics.Models.Fields.Functions;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Math;
    using Accord.Statistics.Models.Markov.Topology;


    [TestClass()]
    public class ConditionalRandomFieldTest
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



        private static HiddenMarkovModel createHMM()
        {
            double[] initial = { 1.0, 0.0 };

            double[,] transitions = 
            {
                { 0.33, 0.66 },
                { 0.00, 1.00 },

            };

            double[,] emissions =
            {
                { 0.25, 0.25, 0.50 },
                { 0.05, 0.05, 0.90 }
            };

            HiddenMarkovModel model = new HiddenMarkovModel(transitions, emissions, initial);
            return model;
        }


        [TestMethod()]
        public void ConditionalRandomFieldConstructorTest()
        {
            HiddenMarkovModel hmm = createHMM();

            int states = 2;
            var function = new MarkovDiscreteFunction(hmm);
            var target = new ConditionalRandomField<int>(states, function);


            Assert.AreEqual(function, target.Function);
            Assert.AreEqual(2, target.States);
        }

        [TestMethod()]
        public void ComputeTest()
        {

            HiddenMarkovModel hmm = HiddenMarkovModelFunctionTest.CreateModel2();

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

        [TestMethod()]
        public void LikelihoodTest()
        {
            HiddenMarkovModel hmm = HiddenMarkovModelFunctionTest.CreateModel2();

            int states = hmm.States;
            int symbols = hmm.Symbols;


            var function1 = new MarkovDiscreteFunction(hmm);
            var target1 = new ConditionalRandomField<int>(states, function1);

            var function2 = new MarkovDiscreteFunction(states, symbols);
            var target2 = new ConditionalRandomField<int>(states, function2);


            int[] observations;

            double a, b, la, lb;

            observations = new int[] { 0, 0, 1, 1, 1, 2 };
            a = target1.LogLikelihood(observations, observations);
            b = target2.LogLikelihood(observations, observations);
            Assert.IsTrue(a > b);

            observations = new int[] { 0, 0, 1, 1, 1, 2 };
            la = target1.LogLikelihood(observations, observations);
            lb = target2.LogLikelihood(observations, observations);
            Assert.IsTrue(la > lb);

            double lla = System.Math.Log(a);
            double llb = System.Math.Log(b);

            Assert.AreEqual(lla, la, 1e-6);
            Assert.AreEqual(llb, lb, 1e-6);
        }

      
    }
}
