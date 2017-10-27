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

#if !NO_BINARY_SERIALIZATION

namespace Accord.Tests.IO
{
    using System;
    using System.IO;
    using System.Text;
    using Accord.IO;
    using NUnit.Framework;
    using Accord.Math;
    using System.Globalization;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.Bayes;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Models.Regression.Linear;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Kernels;
    using Accord.MachineLearning.DecisionTrees;
    using Accord.Statistics.Models.Fields;
    using Accord.Statistics.Models.Markov;

    [TestFixture]
    public class SerializationTest
    {

        [Test]
        public void serialize_batch_models()
        {
#if !MONO
            test(new NaiveBayes(classes: 3, symbols: new[] { 1, 2, 3 }));
            test(new NaiveBayes<NormalDistribution>(classes: 4, inputs: 2, initial: (i, j) => new NormalDistribution(i, j + 1)));
            test(new NaiveBayes<NormalDistribution, double>(classes: 5, inputs: 3, initial: (i, j) => new NormalDistribution(i, j + 1)));
#endif
            test(new LogisticRegression());
            test(new SimpleLinearRegression());
            test(new MultivariateLinearRegression());
            test(new MultipleLinearRegression());

            test(new SupportVectorMachine(inputs: 3));
            test(new SupportVectorMachine<Gaussian>(inputs: 3, kernel: new Gaussian(0.5)));
            test(new SupportVectorMachine<Gaussian, double[]>(inputs: 3, kernel: new Gaussian(0.5)));

            test(new MulticlassSupportVectorMachine(inputs: 3, kernel: new Gaussian(0.5), classes: 2));
            test(new MulticlassSupportVectorMachine<Gaussian, double[]>(inputs: 3, kernel: new Gaussian(0.5), classes: 2));

            test(new MultilabelSupportVectorMachine<Gaussian>(inputs: 3, kernel: new Gaussian(0.5), classes: 2));
            test(new MultilabelSupportVectorMachine<Gaussian, double[]>(inputs: 3, kernel: new Gaussian(0.5), classes: 2));

            test(new DecisionTree(new[] { DecisionVariable.Continuous("test") }, classes: 2));
            test(new HiddenMarkovModel(states: 2, symbols: 3));
            test(new HiddenConditionalRandomField<double>());
        }

        public static void test<T>(T model)
        {
            T actual = Accord.IO.Serializer.DeepClone(model);

            Assert.IsNotNull(actual);
            Assert.AreNotSame(actual, model);
        }
    }
}
#endif
