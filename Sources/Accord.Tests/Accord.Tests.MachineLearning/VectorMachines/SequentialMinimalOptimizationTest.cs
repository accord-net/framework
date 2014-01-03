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

namespace Accord.Tests.MachineLearning
{
    using System;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Math;
    using Accord.Statistics.Kernels;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics.Analysis;

    [TestClass()]
    public class SequentialMinimalOptimizationTest
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
        public void LearnTest()
        {

            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] xor =
            {
                -1,
                 1,
                 1,
                -1
            };

            // Create Kernel Support Vector Machine with a Polynomial Kernel of 2nd degree
            KernelSupportVectorMachine machine = new KernelSupportVectorMachine(new Polynomial(2), inputs[0].Length);

            // Create the sequential minimal optimization teacher
            SequentialMinimalOptimization learn = new SequentialMinimalOptimization(machine, inputs, xor);

            // Run the learning algorithm
            learn.Run();


            int[] output = inputs.Apply(p => Math.Sign(machine.Compute(p)));

            for (int i = 0; i < output.Length; i++)
                Assert.AreEqual(System.Math.Sign(xor[i]), System.Math.Sign(output[i]));
        }

        [TestMethod()]
        public void LearnTest2()
        {

            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] or =
            {
                -1,
                -1,
                -1,
                +1
            };

            // Create Kernel Support Vector Machine with a Polynomial Kernel of 2nd degree
            SupportVectorMachine machine = new SupportVectorMachine(inputs[0].Length);

            // Create the sequential minimal optimization teacher
            SequentialMinimalOptimization learn = new SequentialMinimalOptimization(machine, inputs, or);
            learn.Complexity = 1;

            // Run the learning algorithm
            learn.Run();


            int[] output = inputs.Apply(p => (int)machine.Compute(p));

            for (int i = 0; i < output.Length; i++)
            {
                bool sor = or[i] >= 0;
                bool sou = output[i] >= 0;
                Assert.AreEqual(sor, sou);
            }


        }


        [TestMethod()]
        public void LearnTest3()
        {

            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] xor =
            {
                -1,
                 1,
                 1,
                -1
            };

            // Create Kernel Support Vector Machine with a Polynomial Kernel of 2nd degree
            KernelSupportVectorMachine machine = new KernelSupportVectorMachine(new Polynomial(2), inputs[0].Length);

            // Create the sequential minimal optimization teacher
            SequentialMinimalOptimization learn = new SequentialMinimalOptimization(machine, inputs, xor);

            // Run the learning algorithm
            learn.Run();


            int[] output = inputs.Apply(p => Math.Sign(machine.Compute(p)));

            for (int i = 0; i < output.Length; i++)
                Assert.AreEqual(System.Math.Sign(xor[i]), System.Math.Sign(output[i]));
        }

        [TestMethod()]
        public void LearnTest4()
        {

            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] negatives =
            {
                -1,
                -1,
                -1,
                -1
            };

            // Create Kernel Support Vector Machine with a Polynomial Kernel of 2nd degree
            SupportVectorMachine machine = new SupportVectorMachine(inputs[0].Length);

            // Create the sequential minimal optimization teacher
            SequentialMinimalOptimization learn = new SequentialMinimalOptimization(machine, inputs, negatives);
            learn.Complexity = 1;

            // Run the learning algorithm
            double error = learn.Run();

            Assert.AreEqual(0, error);


            int[] output = inputs.Apply(p => (int)machine.Compute(p));

            for (int i = 0; i < output.Length; i++)
            {
                bool sor = negatives[i] >= 0;
                bool sou = output[i] >= 0;
                Assert.AreEqual(sor, sou);
            }


        }

        [TestMethod()]
        public void LearnTest5()
        {

            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] positives =
            {
                1,
                1,
                1,
                1
            };

            // Create Kernel Support Vector Machine with a Polynomial Kernel of 2nd degree
            SupportVectorMachine machine = new SupportVectorMachine(inputs[0].Length);

            // Create the sequential minimal optimization teacher
            SequentialMinimalOptimization learn = new SequentialMinimalOptimization(machine, inputs, positives);
            learn.Complexity = 1;

            // Run the learning algorithm
            double error = learn.Run();

            Assert.AreEqual(0, error);


            int[] output = inputs.Apply(p => (int)machine.Compute(p));

            for (int i = 0; i < output.Length; i++)
            {
                bool sor = positives[i] >= 0;
                bool sou = output[i] >= 0;
                Assert.AreEqual(sor, sou);
            }
        }

        [TestMethod, Ignore]
        public void LargeLearningTest1()
        {
            // Create large input vectors

            int rows = 1000;
            int dimension = 10000;

            double[][] inputs = new double[rows][];
            int[] outputs = new int[rows];

            Random rnd = new Random();

            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = new double[dimension];

                if (i > rows / 2)
                {
                    for (int j = 0; j < dimension; j++)
                        inputs[i][j] = rnd.NextDouble();
                    outputs[i] = -1;
                }
                else
                {
                    for (int j = 0; j < dimension; j++)
                        inputs[i][j] = rnd.NextDouble() * 4.21 + 5;
                    outputs[i] = +1;
                }
            }

            KernelSupportVectorMachine svm = new KernelSupportVectorMachine(new Polynomial(2), dimension);

            SequentialMinimalOptimization smo = new SequentialMinimalOptimization(svm, inputs, outputs)
            {
                UseComplexityHeuristic = true
            };


            double error = smo.Run();

            Assert.AreEqual(0, error);
        }

        [TestMethod()]
        public void SequentialMinimalOptimizationConstructorTest()
        {
            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] or =
            {
                0,
                0,
                0,
                +1
            };

            // Create Kernel Support Vector Machine with a Polynomial Kernel of 2nd degree
            SupportVectorMachine machine = new SupportVectorMachine(inputs[0].Length);

            bool thrown = false;
            try
            {
                SequentialMinimalOptimization learn = new SequentialMinimalOptimization(machine, inputs, or);
            }
            catch (ArgumentOutOfRangeException)
            {
                thrown = true;
            }

            Assert.IsTrue(thrown);
        }


        [TestMethod()]
        public void ComputeTest5()
        {
            var dataset = yinyang;

            double[][] inputs = dataset.Submatrix(null, 0, 1).ToArray();
            int[] labels = dataset.GetColumn(2).ToInt32();

            {
                Linear kernel = new Linear();
                var machine = new KernelSupportVectorMachine(kernel, inputs[0].Length);
                var smo = new SequentialMinimalOptimization(machine, inputs, labels);

                smo.Complexity = 1.0;

                double error = smo.Run();

                Assert.AreEqual(1.0, smo.Complexity);
                Assert.AreEqual(1.0, smo.WeightRatio);
                Assert.AreEqual(1.0, smo.NegativeWeight);
                Assert.AreEqual(1.0, smo.PositiveWeight);
                Assert.AreEqual(0.14, error);
                Assert.AreEqual(30, machine.SupportVectors.Length);

                double[] actualWeights = machine.Weights;
                double[] expectedWeights = { -1, -1, 1, -1, -1, 1, 1, -1, 1, -1, 1, 1, -1, 0.337065120144639, -1, 1, -0.337065120144639, -1, 1, 1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1 };
                Assert.IsTrue(expectedWeights.IsEqual(actualWeights, 1e-10));

                int[] actual = new int[labels.Length];
                for (int i = 0; i < actual.Length; i++)
                    actual[i] = Math.Sign(machine.Compute(inputs[i]));

                ConfusionMatrix matrix = new ConfusionMatrix(actual, labels);

                Assert.AreEqual(7, matrix.FalseNegatives);
                Assert.AreEqual(7, matrix.FalsePositives);
                Assert.AreEqual(43, matrix.TruePositives);
                Assert.AreEqual(43, matrix.TrueNegatives);
            }

            {
                Linear kernel = new Linear();
                var machine = new KernelSupportVectorMachine(kernel, inputs[0].Length);
                var smo = new SequentialMinimalOptimization(machine, inputs, labels);

                smo.Complexity = 1.0;
                smo.PositiveWeight = 0.3;
                smo.NegativeWeight = 1.0;

                double error = smo.Run();

                Assert.AreEqual(1.0, smo.Complexity);
                Assert.AreEqual(0.3 / 1.0, smo.WeightRatio);
                Assert.AreEqual(1.0, smo.NegativeWeight);
                Assert.AreEqual(0.3, smo.PositiveWeight);
                Assert.AreEqual(0.21, error);
                Assert.AreEqual(24, machine.SupportVectors.Length);

                double[] actualWeights = machine.Weights;
                //string str = actualWeights.ToString(Accord.Math.Formats.CSharpArrayFormatProvider.InvariantCulture);
                double[] expectedWeights = { -0.771026323762095, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -0.928973676237905, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
                Assert.IsTrue(expectedWeights.IsEqual(actualWeights, 1e-10));

                int[] actual = new int[labels.Length];
                for (int i = 0; i < actual.Length; i++)
                    actual[i] = (int)machine.Compute(inputs[i]);

                ConfusionMatrix matrix = new ConfusionMatrix(actual, labels);

                Assert.AreEqual(50, matrix.FalseNegatives);
                Assert.AreEqual(0, matrix.FalsePositives);
                Assert.AreEqual(0, matrix.TruePositives);
                Assert.AreEqual(50, matrix.TrueNegatives);
            }

            {
                Linear kernel = new Linear();
                var machine = new KernelSupportVectorMachine(kernel, inputs[0].Length);
                var smo = new SequentialMinimalOptimization(machine, inputs, labels);

                smo.Complexity = 1.0;
                smo.PositiveWeight = 1.0;
                smo.NegativeWeight = 0.3;

                double error = smo.Run();

                Assert.AreEqual(1.0, smo.Complexity);
                Assert.AreEqual(1.0 / 0.3, smo.WeightRatio);
                Assert.AreEqual(0.3, smo.NegativeWeight);
                Assert.AreEqual(1.0, smo.PositiveWeight);
                Assert.AreEqual(0.15, error);
                Assert.AreEqual(19, machine.SupportVectors.Length);

                double[] actualWeights = machine.Weights;
                double[] expectedWeights = new double[] { 1, 1, -0.3, 1, -0.3, 1, 1, -0.3, 1, 1, 1, 1, 1, 1, 1, 1, 0.129080057278249, 1, 0.737797469918795 };
                Assert.IsTrue(expectedWeights.IsEqual(actualWeights, 1e-10));

                int[] actual = new int[labels.Length];
                for (int i = 0; i < actual.Length; i++)
                    actual[i] = Math.Sign(machine.Compute(inputs[i]));

                ConfusionMatrix matrix = new ConfusionMatrix(actual, labels);

                Assert.AreEqual(0, matrix.FalseNegatives);
                Assert.AreEqual(50, matrix.FalsePositives);
                Assert.AreEqual(50, matrix.TruePositives);
                Assert.AreEqual(0, matrix.TrueNegatives);
            }
        }

        [TestMethod()]
        public void CompactTest()
        {

            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] xor =
            {
                -1,
                 1,
                 1,
                -1
            };

            {
                var machine = new KernelSupportVectorMachine(new Polynomial(2), inputs[0].Length);
                var learn = new SequentialMinimalOptimization(machine, inputs, xor);

                learn.Compact = false;
                double error = learn.Run();
                Assert.AreEqual(0, error);
            }

            {
                var machine = new KernelSupportVectorMachine(new Polynomial(2), inputs[0].Length);
                var learn = new SequentialMinimalOptimization(machine, inputs, xor);

                bool thrown = false;
                try { learn.Compact = true; }
                catch { thrown = true; }
                Assert.IsTrue(thrown);
            }

            {
                var machine = new KernelSupportVectorMachine(new Linear(), inputs[0].Length);
                var learn = new SequentialMinimalOptimization(machine, inputs, xor);

                learn.Compact = false;
                double error = learn.Run();
                Assert.AreEqual(0.5, error);
            }

            {
                var machine = new KernelSupportVectorMachine(new Linear(), inputs[0].Length);
                var learn = new SequentialMinimalOptimization(machine, inputs, xor);

                learn.Compact = false;
                double error = learn.Run();
                Assert.AreEqual(0.5, error);
            }
        }


        public static double[,] yinyang =
        {
            #region Yin Yang
            { -0.876847428, 1.996318824, -1 },
            { -0.748759325, 1.997248514, -1 },
            { -0.635574695, 1.978046579, -1 },
            { -0.513769071, 1.973224777, -1 },
            { -0.382577547, 1.955077224, -1 },
            { -0.275144211, 1.923813789, -1 },
            { -0.156802752, 1.949219695, -1 },
            { -0.046002059, 1.895342542, -1 },
            { 0.084152257, 1.873104082, -1 },
            { 0.192063131, 1.868157532, -1 },
            { 0.238547032, 1.811664165, -1 },
            { 0.381412694, 1.830869925, -1 },
            { 0.431182119, 1.755312479, -1 },
            { 0.562589082, 1.725444806, -1 },
            { 0.553294269, 1.689047886, -1 },
            { 0.730976261, 1.610522064, -1 },
            { 0.722164981, 1.633112952, -1 },
            { 0.861069302, 1.562450197, -1 },
            { 0.825107945, 1.435846225, -1 },
            { 0.825261132, 1.456391196, -1 },
            { 0.948721626, 1.393367552, -1 },
            { 1.001705278, 1.275768447, -1 },
            { 0.966788667, 1.321375233, -1 },
            { 1.030828944, 1.228437023, -1 },
            { 1.083195636, 1.143011589, -1 },
            { 0.920876422, 1.037854388, -1 },
            { 0.994518277, 1.064971023, -1 },
            { 0.954169422, 0.938084211, -1 },
            { 0.903586083, 0.985255341, -1 },
            { 0.877869854, 0.729143525, -1 },
            { 0.866594018, 0.75025734, -1 },
            { 0.757278389, 0.638917822, -1 },
            { 0.655489515, 0.670717406, -1 },
            { 0.687639626, 0.511655563, -1 },
            { 0.656365078, 0.638542346, -1 },
            { 0.491775914, 0.401874802, -1 },
            { 0.35504489, 0.38963967, -1 },
            { 0.275616568, 0.182958126, -1 },
            { 0.338471037, 0.102347682, -1 },
            { 0.103918095, 0.152960961, -1 },
            { 0.238473941, -0.070899965, -1 },
            { -0.00657754, 0.168107931, -1 },
            { -0.091307058, -0.032174399, -1 },
            { -0.290772034, -0.345025689, -1 },
            { -0.287555253, -0.397984323, -1 },
            { -0.363424618, -0.365636808, -1 },
            { -0.544071691, -0.512970644, -1 },
            { -0.7098968, -0.54654921, -1 },
            { -1.007857216, -0.811837224, -1 },
            { -0.932787122, -0.687973276, -1 },
            { -0.123987649, -1.547976483, 1 },
            { -0.247236701, -1.546629461, 1 },
            { -0.369357682, -1.533968755, 1 },
            { -0.497892178, -1.525597952, 1 },
            { -0.606998699, -1.518386229, 1 },
            { -0.751556976, -1.46427032, 1 },
            { -0.858848619, -1.464142289, 1 },
            { -0.957834238, -1.454165888, 1 },
            { -1.061602698, -1.444783216, 1 },
            { -1.169634343, -1.426033507, 1 },
            { -1.272115895, -1.408678817, 1 },
            { -1.380383293, -1.345651442, 1 },
            { -1.480866574, -1.279955202, 1 },
            { -1.548927664, -1.223262541, 1 },
            { -1.597886819, -1.227115936, 1 },
            { -1.686711497, -1.141898276, 1 },
            { -1.812689051, -1.14805053, 1 },
            { -1.809841336, -1.083347602, 1 },
            { -1.938850711, -1.019723742, 1 },
            { -1.974552679, -0.970515422, 1 },
            { -1.953184359, -0.88363121, 1 },
            { -1.98749965, -0.861879772, 1 },
            { -2.04215554, -0.797813815, 1 },
            { -1.984185734, -0.826986835, 1 },
            { -2.063307605, -0.749495213, 1 },
            { -1.964274134, -0.653639779, 1 },
            { -2.020258155, -0.530431615, 1 },
            { -1.946081996, -0.514425683, 1 },
            { -1.934356006, -0.435380423, 1 },
            { -1.827017658, -0.425058004, 1 },
            { -1.788385889, -0.312443513, 1 },
            { -1.800874033, -0.237312969, 1 },
            { -1.784225126, 0.013987951, 1 },
            { -1.682828321, -0.063911465, 1 },
            { -1.754042471, -0.075520653, 1 },
            { -1.5680733, 0.110795036, 1 },
            { -1.438333268, 0.170230561, 1 },
            { -1.356614661, 0.163613841, 1 },
            { -1.336362397, 0.334537756, 1 },
            { -1.296677607, 0.316006907, 1 },
            { -1.109908857, 0.474036646, 1 },
            { -0.845929174, 0.485303884, 1 },
            { -0.855794711, 0.395603118, 1 },
            { -0.68479255, 0.671166245, 1 },
            { -0.514222252, 0.652065554, 1 },
            { -0.387612557, 0.700858902, 1 },
            { -0.51939719, 1.025735335, 1 },
            { -0.228760025, 0.93490314, 1 },
            { -0.293782477, 1.008861678, 1 },
            { 0.013431012, 1.082021525, 1 },
            #endregion
        };
    }
}
