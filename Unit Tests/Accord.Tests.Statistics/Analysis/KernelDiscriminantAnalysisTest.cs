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
    using Accord.Statistics.Kernels;
    using Accord.Math;
    using System;
    using IO;
    using Accord.MachineLearning;
    using Accord.Statistics.Models.Regression;

    [TestFixture]
    public class KernelDiscriminantAnalysisTest
    {
        [Test]
        public void ClassifyTest()
        {
            // Create some sample input data instances. This is the same
            // data used in the Gutierrez-Osuna's example available on:
            // http://research.cs.tamu.edu/prism/lectures/pr/pr_l10.pdf

            double[][] inputs =
            {
                // Class 0
                new double[] {  4,  1 },
                new double[] {  2,  4 },
                new double[] {  2,  3 },
                new double[] {  3,  6 },
                new double[] {  4,  4 },

                // Class 1
                new double[] {  9, 10 },
                new double[] {  6,  8 },
                new double[] {  9,  5 },
                new double[] {  8,  7 },
                new double[] { 10,  8 }
            };

            int[] output =
            {
                0, 0, 0, 0, 0, // The first five are from class 0
                1, 1, 1, 1, 1  // The last five are from class 1
            };

            // Now we can chose a kernel function to 
            // use, such as a linear kernel function.
            IKernel kernel = new Linear();

            // Then, we will create a KDA using this linear kernel.
            var kda = new KernelDiscriminantAnalysis(inputs, output, kernel);

            kda.Compute(); // Compute the analysis


            // Now we can project the data into KDA space:
            double[][] projection = kda.Transform(inputs);

            double[][] classifierProjection = kda.Classifier.First.Transform(inputs);
            Assert.IsTrue(projection.IsEqual(classifierProjection));

            // Or perform classification using:
            int[] results = kda.Classify(inputs);

            string str = projection.ToCSharp();

            double[][] expected = new double[][] {
                new double[] { 80.7607049998409, -5.30485371541545E-06, 6.61304584781419E-06, 4.52807990036774E-06, -3.44409628150189E-06, 3.69094504515388E-06, -1.33641000168438E-05, -0.000132874977040842, -0.000261921590627878, 1.22137997452386 },
                new double[] { 67.6629612351861, 6.80622743409742E-06, -8.48466262226566E-06, -5.80961187779394E-06, 4.4188405141643E-06, -4.73555212510135E-06, 1.71463925084936E-05, 0.000170481102685471, 0.000336050342774286, -1.5670535522193 },
                new double[] { 59.8679301679674, 4.10375477777336E-06, -5.11575246520124E-06, -3.50285421113483E-06, 2.66430090034575E-06, -2.85525936627451E-06, 1.03382660725515E-05, 0.00010279007663172, 0.000202618589039361, -0.944841112367518 },
                new double[] { 101.494441852779, 1.02093411395998E-05, -1.27269939227403E-05, -8.71441780958548E-06, 6.62826077091339E-06, -7.10332818965043E-06, 2.57195887591877E-05, 0.000255721654028207, 0.000504075514164981, -2.35058032832894 },
                new double[] { 104.145798201497, 2.80256425000402E-06, -3.49368461627364E-06, -2.39219308895144E-06, 1.81952256639306E-06, -1.94993321933623E-06, 7.06027928387698E-06, 7.01981011275166E-05, 0.000138373670580449, -0.645257345031474 },
                new double[] { 242.123077020588, 9.00824221261587E-06, -1.12297005614437E-05, -7.689192102589E-06, 5.84846541151762E-06, -6.26764250277745E-06, 2.26937548148953E-05, 0.000225636753569347, 0.000444772512580016, -2.07404146617259 },
                new double[] { 171.808759436683, 9.60879168943052E-06, -1.19783472456447E-05, -8.2018049702981E-06, 6.23836308744075E-06, -6.68548535731617E-06, 2.42066717959233E-05, 0.000240679203812988, 0.000474424013376051, -2.21231089725078 },
                new double[] { 203.147921684494, -4.5041210583463E-06, 5.61485022387842E-06, 3.8445962076139E-06, -2.92423269243614E-06, 3.13382127359318E-06, -1.13468773577097E-05, -0.000112818376692303, -0.000222386256126583, 1.03702073308629 },
                new double[] { 200.496565335776, 2.90265583302585E-06, -3.61845908969372E-06, -2.47762852723099E-06, 1.88450551963371E-06, -2.01957368695105E-06, 7.31243213181187E-06, 7.27051762225983E-05, 0.000143315587422421, -0.668302250211177 },
                new double[] { 244.774433369306, 1.60146531058558E-06, -1.99639123366069E-06, -1.36696743169296E-06, 1.0397271781315E-06, -1.11424755644407E-06, 4.03444536090092E-06, 4.01132006970784E-05, 7.90706689741683E-05, -0.368718482875124 }
            };

            Assert.IsTrue(expected.IsEqual(projection, 1e-6));

            // Test the classify method
            for (int i = 0; i < 5; i++)
            {
                int actual = results[i];
                Assert.AreEqual(0, actual);
            }

            for (int i = 5; i < 10; i++)
            {
                int actual = results[i];
                Assert.AreEqual(1, actual);
            }
        }

        [Test]
        public void learn_test()
        {
            #region doc_learn
            // Create some sample input data instances. This is the same
            // data used in the Gutierrez-Osuna's example available on:
            // http://research.cs.tamu.edu/prism/lectures/pr/pr_l10.pdf

            double[][] inputs =
            {
                // Class 0
                new double[] {  4,  1 },
                new double[] {  2,  4 },
                new double[] {  2,  3 },
                new double[] {  3,  6 },
                new double[] {  4,  4 },

                // Class 1
                new double[] {  9, 10 },
                new double[] {  6,  8 },
                new double[] {  9,  5 },
                new double[] {  8,  7 },
                new double[] { 10,  8 }
            };

            int[] output =
            {
                0, 0, 0, 0, 0, // The first five are from class 0
                1, 1, 1, 1, 1  // The last five are from class 1
            };


            // We'll create a KDA using a Linear kernel
            var kda = new KernelDiscriminantAnalysis()
            {
                Kernel = new Linear() // We can choose any kernel function
            };

            // Compute the analysis and create a classifier
            var classifier = kda.Learn(inputs, output);

            // Now we can project the data into KDA space:
            double[][] projection = kda.Transform(inputs);

            // Or perform classification using:
            int[] results = kda.Classify(inputs);

            // Note: The classifier generated by the KDA analysis is composed
            // of a two-step transformation. The first transformation projects 
            // the input data into a new space using a kernel regression:
            MultivariateKernelRegression kernelRegression = classifier.First;
            // While the second is a classifier that attempts to map the outputs
            // of the kernel regression to each class according to their average:
            MinimumMeanDistanceClassifier meanClassifier = classifier.Second;

            // As such, calling kda.Classify is equivalent to calling:
            int[] results2 = classifier.Decide(inputs);

            // which in turn is equivalent to calling:
            int[] results3 = meanClassifier.Decide(kernelRegression.Transform(inputs));
            #endregion

            Assert.AreEqual(results, results2);
            Assert.AreEqual(results, results3);

            double[][] classifierProjection = kda.Classifier.First.Transform(inputs);
            Assert.IsTrue(projection.IsEqual(classifierProjection));

            double[][] expected = new double[][] {
                new double[] { 80.7607049998409, -5.30485371541545E-06, 6.61304584781419E-06, 4.52807990036774E-06, -3.44409628150189E-06, 3.69094504515388E-06, -1.33641000168438E-05, -0.000132874977040842, -0.000261921590627878, 1.22137997452386 },
                new double[] { 67.6629612351861, 6.80622743409742E-06, -8.48466262226566E-06, -5.80961187779394E-06, 4.4188405141643E-06, -4.73555212510135E-06, 1.71463925084936E-05, 0.000170481102685471, 0.000336050342774286, -1.5670535522193 },
                new double[] { 59.8679301679674, 4.10375477777336E-06, -5.11575246520124E-06, -3.50285421113483E-06, 2.66430090034575E-06, -2.85525936627451E-06, 1.03382660725515E-05, 0.00010279007663172, 0.000202618589039361, -0.944841112367518 },
                new double[] { 101.494441852779, 1.02093411395998E-05, -1.27269939227403E-05, -8.71441780958548E-06, 6.62826077091339E-06, -7.10332818965043E-06, 2.57195887591877E-05, 0.000255721654028207, 0.000504075514164981, -2.35058032832894 },
                new double[] { 104.145798201497, 2.80256425000402E-06, -3.49368461627364E-06, -2.39219308895144E-06, 1.81952256639306E-06, -1.94993321933623E-06, 7.06027928387698E-06, 7.01981011275166E-05, 0.000138373670580449, -0.645257345031474 },
                new double[] { 242.123077020588, 9.00824221261587E-06, -1.12297005614437E-05, -7.689192102589E-06, 5.84846541151762E-06, -6.26764250277745E-06, 2.26937548148953E-05, 0.000225636753569347, 0.000444772512580016, -2.07404146617259 },
                new double[] { 171.808759436683, 9.60879168943052E-06, -1.19783472456447E-05, -8.2018049702981E-06, 6.23836308744075E-06, -6.68548535731617E-06, 2.42066717959233E-05, 0.000240679203812988, 0.000474424013376051, -2.21231089725078 },
                new double[] { 203.147921684494, -4.5041210583463E-06, 5.61485022387842E-06, 3.8445962076139E-06, -2.92423269243614E-06, 3.13382127359318E-06, -1.13468773577097E-05, -0.000112818376692303, -0.000222386256126583, 1.03702073308629 },
                new double[] { 200.496565335776, 2.90265583302585E-06, -3.61845908969372E-06, -2.47762852723099E-06, 1.88450551963371E-06, -2.01957368695105E-06, 7.31243213181187E-06, 7.27051762225983E-05, 0.000143315587422421, -0.668302250211177 },
                new double[] { 244.774433369306, 1.60146531058558E-06, -1.99639123366069E-06, -1.36696743169296E-06, 1.0397271781315E-06, -1.11424755644407E-06, 4.03444536090092E-06, 4.01132006970784E-05, 7.90706689741683E-05, -0.368718482875124 }
            };

            Assert.IsTrue(expected.Get(null, 0, 2).IsEqual(projection, 1e-6));

            // Test the classify method
            for (int i = 0; i < 5; i++)
            {
                int actual = results[i];
                Assert.AreEqual(0, actual);
            }

            for (int i = 5; i < 10; i++)
            {
                int actual = results[i];
                Assert.AreEqual(1, actual);
            }
        }


        [Test]
        public void ClassifyTest1()
        {
            // Create some sample input data

            // This is the same data used in the example by Gutierrez-Osuna
            // http://research.cs.tamu.edu/prism/lectures/pr/pr_l10.pdf

            double[,] inputs =
            {
                {  4,  1 }, // Class 1
                {  2,  4 },
                {  2,  3 },
                {  3,  6 },
                {  4,  4 },

                {  9, 10 }, // Class 2
                {  6,  8 },
                {  9,  5 },
                {  8,  7 },
                { 10,  8 }
            };

            int[] output =
            {
                1, 1, 1, 1, 1, // Class labels for the input vectors
                2, 2, 2, 2, 2
            };

            // Create a new Linear Discriminant Analysis object
            var lda = new KernelDiscriminantAnalysis(inputs, output, new Linear());

            // Compute the analysis
            lda.Compute();

            // Test the classify method
            for (int i = 0; i < 5; i++)
            {
                int expected = 1;
                int actual = lda.Classify(inputs.GetRow(i));
                Assert.AreEqual(expected, actual);
            }

            for (int i = 5; i < 10; i++)
            {
                int expected = 2;
                int actual = lda.Classify(inputs.GetRow(i));
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void ConstructorTest1()
        {
            double[,] inputs =
            {
                { 1, 1, },
                { 2, 2, },
                { 3, 3, },
            };

            int[] output = { 1, 2, 3 };

            IKernel kernel = new Gaussian(0.1);

            var target = new KernelDiscriminantAnalysis(inputs, output, kernel);

            Assert.AreEqual(3, target.Classes.Count);
            Assert.AreEqual(0, target.Classes[0].Index);
            Assert.AreEqual(1, target.Classes[1].Index);
            Assert.AreEqual(2, target.Classes[2].Index);
            Assert.AreEqual(1, target.Classes[0].Number);
            Assert.AreEqual(2, target.Classes[1].Number);
            Assert.AreEqual(3, target.Classes[2].Number);

            Assert.AreEqual(output, target.Classifications);
            Assert.AreEqual(kernel, target.Kernel);
            Assert.AreEqual(1e-4, target.Regularization);
            Assert.AreEqual(inputs, target.Source);
            //Assert.AreEqual(0.001, target.Threshold);

            Assert.IsNull(target.CumulativeProportions);
            Assert.IsNull(target.DiscriminantMatrix);
            Assert.IsNull(target.DiscriminantProportions);
            Assert.IsNull(target.Discriminants);
            Assert.IsNull(target.Eigenvalues);
            Assert.IsNull(target.Result);
            Assert.IsNull(target.ScatterBetweenClass);
            Assert.IsNull(target.ScatterMatrix);
            Assert.IsNull(target.ScatterWithinClass);
            Assert.IsNull(target.StandardDeviations);
            Assert.IsNull(target.Means);
        }

        [Test]
        public void ComputeTest()
        {
            double[,] inputs =
            {
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 },
            };

            int[] output = { 0, 1 };

            IKernel kernel = new Gaussian(0.1);
            var target = new KernelDiscriminantAnalysis(inputs, output, kernel);
            target.Threshold = 0;

            target.Compute();

            double[,] actual = target.Transform(inputs);

            double[,] expected =
            {
                {  1.0, -1.0 },
                { -1.0, -1.0 },
            };

            Assert.IsTrue(Matrix.IsEqual(actual, expected));
        }

        [Test]
        public void ComputeTest2()
        {
            // Schölkopf KPCA toy example
            double[,] inputs = scholkopf();

            int[] output = Matrix.Expand(new int[,] { { 1 }, { 2 }, { 3 } },
                    new int[] { 30, 30, 30 }).GetColumn(0);

            IKernel kernel = new Gaussian(0.2);
            var target = new KernelDiscriminantAnalysis(inputs, output, kernel);

            target.Compute();


            double[,] actual = target.Transform(inputs, 2);

            double[,] expected1 =
            {
                { 1.2785801485080475, 0.20539157505913622},
                { 1.2906613255489541, 0.20704272225753775},
                { 1.2978134597266808, 0.20802649628632208},
            };

            double[,] actual1 = actual.Submatrix(0, 2, 0, 1);

            Assert.IsTrue(Matrix.IsEqual(actual1, expected1, 0.0000001));

            // Assert the result equals the transformation of the input
            double[,] result = target.Result;
            double[,] projection = target.Transform(inputs);
            Assert.IsTrue(Matrix.IsEqual(result, projection));
        }

        public static double[,] scholkopf()
        {
            double[,] inputs =
            {
               #region Scholkopf KPCA Toy Example
                { -0.383504649, -0.162495898 },  { -0.437316092, -0.087483818 },
                { -0.492491985, -0.127135841 },  { -0.46483931,  -0.437745429 },
                { -0.569651254, -0.227378242 },  { -0.330385752, -0.232293992 },
                { -0.494094022, -0.168201208 },  { -0.320292822, -0.251117221 },
                { -0.473593147, -0.200204135 },  { -0.412832671, -0.039348904 },
                { -0.644617154, -0.115235137 },  { -0.570116535, -0.173189919 },
                { -0.375401788, -0.292348909 },  { -0.5638977,   -0.207049939 },
                { -0.442264978, -0.185210865 },  { -0.536002963, -0.255709364 },
                { -0.513557629, -0.23367057  },  { -0.634933848, -0.158477254 },
                { -0.62704499,  -0.044218646 },  { -0.401542973, -0.44442989  },
                { -0.504488061, -0.309819539 },  { -0.579894452, -0.087735214 },
                { -0.576517243, -0.141833274 },  { -0.41382651,  -0.22713543  },
                { -0.505622512, -0.158580869 },  { -0.448652183, -0.297781423 },
                { -0.460331913, -0.302146617 },  { -0.424378103, -0.168231202 },
                { -0.459951398, -0.04838922  },  { -0.634138072, -0.125056755 },
                { -0.050770039,  0.603988485 },  {  0.088529945,  0.351715749 },
                { -0.024809355,  0.715865471 },  { -0.0726249,    0.497372053 },
                { -0.04450403,   0.715348699 },  { -0.061291112,  0.521354339 },
                { -0.020914408,  0.663480859 },  {  0.056214783,  0.682040976 },
                { -0.106392289,  0.582397349 },  {  0.035158895,  0.656247387 },
                {  0.113299993,  0.587255712 },  {  0.014999425,  0.655417156 },
                {  0.070314405,  0.490265568 },  { -0.005241158,  0.52686986  },
                {  0.201849612,  0.740473192 },  {  0.09241594,   0.537978579 },
                { -0.18141147,   0.623714877 },  {  0.003497332,  0.441315301 },
                { -0.180786206,  0.559851519 },  {  0.102819255,  0.522930773 },
                {  0.039460031,  0.573731949 },  {  0.063940564,  0.697648954 },
                {  0.087421289,  0.697781504 },  {  0.175240173,  0.717002111 },
                { -0.032005083,  0.615931086 },  { -0.013741381,  0.649952085 },
                {  0.061576963,  0.494462493 },  {  0.097789407,  0.55492568  },
                { -0.111534771,  0.727037824 },  { -0.055002145,  0.68986936  },
                {  0.54387051,   0.05261625  },  {  0.375265568, -0.018445412 },
                {  0.532466692,  0.019878283 },  {  0.539007041,  0.159042684 },
                {  0.459486168,  0.003219164 },  {  0.529231488,  0.088916367 },
                {  0.756591024, -0.129915249 },  {  0.454218436,  0.11825731  },
                {  0.338917299,  0.181747171 },  {  0.233047622, -0.058430213 },
                {  0.424030335, -0.101067382 },  {  0.432527914, -0.096049831 },
                {  0.382831281,  0.069115958 },  {  0.703293002, -0.075861821 },
                {  0.596848105, -0.009697173 },  {  0.5670292,   -0.140694905 },
                {  0.542014604,  0.103081246 },  {  0.212724873, -0.07598744  },
                {  0.668587408,  0.087412723 },  {  0.502792455,  0.0761127   },
                {  0.409796942, -0.016592345 },  {  0.294674251,  0.030090744 },
                {  0.50890863,  -0.032246733 },  {  0.708709913, -0.036841128 },
                {  0.536511846,  0.114789528 },  {  0.584610553,  0.004143026 },
                {  0.481546234, -0.109804965 },  {  0.603071442,  0.156672375 },
                {  0.347237735, -0.104842345 },  {  0.596493896,  0.042272368 },
#endregion
            };
            return inputs;
        }

        [Test]
        public void ComputeTest3()
        {
            // Schölkopf KPCA toy example
            double[][] inputs = scholkopf().ToJagged();

            int[] output = Matrix.Expand(new int[,] { { 1 }, { 2 }, { 3 } }, new int[] { 30, 30, 30 }).GetColumn(0);

            IKernel kernel = new Gaussian(0.2);
            var target = new KernelDiscriminantAnalysis(inputs, output, kernel);

            target.Compute();


            double[][] actual = target.Transform(inputs, 2);

            double[][] expected1 =
            {
                new double[] { 1.2785801485080475, 0.20539157505913622},
                new double[] { 1.2906613255489541, 0.20704272225753775},
                new double[] { 1.2978134597266808, 0.20802649628632208},
            };

            double[][] actual1 = actual.Submatrix(0, 2, 0, 1);

            Assert.IsTrue(Matrix.IsEqual(actual1, expected1, 0.0000001));

            // Assert the result equals the transformation of the input
            double[][] result = target.Result.ToJagged();
            double[][] projection = target.Transform(inputs);
            Assert.IsTrue(Matrix.IsEqual(result, projection));

            int[] actual2 = target.Classify(inputs);
            Assert.IsTrue(Matrix.IsEqual(actual2, output));

            int[] actual3 = new int[inputs.Length];
            double[][] scores = new double[inputs.Length][];
            for (int i = 0; i < inputs.Length; i++)
                actual3[i] = target.Classify(inputs[i], out scores[i]);
            Assert.IsTrue(Matrix.IsEqual(actual3, output));

            scores = scores.Get(0, 5, null);

            double[][] expected = new double[][] {
                new double[] { -6.23928931356786E-06, -5.86731829543872, -4.76988430445096 },
                new double[] { -9.44593697210785E-05, -5.92312597750504, -4.82189359956088 },
                new double[] { -0.000286839977573986, -5.95629842504978, -4.85283341267476 },
                new double[] { -4.38986003009456E-05, -5.84990179343448, -4.75189423787298 },
                new double[] { -0.000523817959022851, -5.77534144986199, -4.683120454667 }
            };

            //Assert.IsTrue(Matrix.IsEqual(scores, expected, 1e-6));
        }

        [Test]
        public void scholkopf_new_method()
        {
            // Schölkopf KPCA toy example
            double[][] inputs = scholkopf().ToJagged();

            int[] output = Matrix.Expand(new int[,] { { 0 }, { 1 }, { 2 } }, new int[] { 30, 30, 30 }).GetColumn(0);

            IKernel kernel = new Gaussian(0.2);
            var target = new KernelDiscriminantAnalysis(kernel);

            var cls = target.Learn(inputs, output);


            double[][] actual = target.Transform(inputs, 2);

            double[][] expected1 =
            {
                new double[] { 1.2785801485080475, 0.20539157505913622},
                new double[] { 1.2906613255489541, 0.20704272225753775},
                new double[] { 1.2978134597266808, 0.20802649628632208},
            };

            double[][] actual1 = actual.Submatrix(0, 2, 0, 1);

            Assert.IsTrue(Matrix.IsEqual(actual1, expected1, 0.0000001));
            Assert.IsNull(target.Result);

            int[] actual2 = target.Classify(inputs);
            Assert.IsTrue(Matrix.IsEqual(actual2, output));

            int[] actual4 = cls.Decide(inputs);
            Assert.IsTrue(Matrix.IsEqual(actual4, output));

            int[] actual3 = new int[inputs.Length];
            double[][] scores = new double[inputs.Length][];
            for (int i = 0; i < inputs.Length; i++)
                actual3[i] = target.Classify(inputs[i], out scores[i]);
            Assert.IsTrue(Matrix.IsEqual(actual3, output));

            scores = scores.Get(0, 5, null);

            double[][] expected = new double[][] {
                new double[] { -6.23928931356786E-06, -5.86731829543872, -4.76988430445096 },
                new double[] { -9.44593697210785E-05, -5.92312597750504, -4.82189359956088 },
                new double[] { -0.000286839977573986, -5.95629842504978, -4.85283341267476 },
                new double[] { -4.38986003009456E-05, -5.84990179343448, -4.75189423787298 },
                new double[] { -0.000523817959022851, -5.77534144986199, -4.683120454667 }
            };

            Assert.IsTrue(Matrix.IsEqual(scores, expected, 1e-6));
        }

        [Test]
        public void large_transform_few_components()
        {
            int n = 100;
            double[][] data = Jagged.Random(n, n);
            int[] labels = Vector.Random(n, 0, 10);

            var kda = new KernelDiscriminantAnalysis();
            var target = kda.Learn(data, labels);

            var expected = kda.Transform(data, 2);
            Assert.AreEqual(n, expected.Rows());
            Assert.AreEqual(2, expected.Columns());

            kda.NumberOfOutputs = 2;
            target = kda.Learn(data, labels);

            var actual = target.First.Transform(data);
            Assert.AreEqual(n, actual.Rows());
            Assert.AreEqual(2, actual.Columns());

            Assert.IsTrue(actual.IsEqual(expected));
        }

        [Test]
        public void ThresholdTest()
        {
            double[,] inputs =
            {
                { 1 },
                { 2 },
                { 3 },
            };

            int[] output = { 0, 1, 1 };

            IKernel kernel = new Gaussian(0.1);
            var target = new KernelDiscriminantAnalysis(inputs, output, kernel);

            bool thrown = false;
            try { target.Threshold = -1; }
            catch (ArgumentOutOfRangeException) { thrown = true; }
            Assert.IsTrue(thrown);

            thrown = false;
            try { target.Threshold = 1.1; }
            catch (ArgumentOutOfRangeException) { thrown = true; }
            Assert.IsTrue(thrown);

            target.Threshold = 1.0;

            target.Compute();

            Assert.AreEqual(2, target.Classes.Count);
            Assert.AreEqual(0, target.Classes[0].Number);
            Assert.AreEqual(1, target.Classes[0].Indices.Length);
            Assert.AreEqual(0, target.Classes[0].Indices[0]);
            Assert.AreEqual(1, target.Classes[1].Number);
            Assert.AreEqual(2, target.Classes[1].Indices.Length);
            Assert.AreEqual(1, target.Classes[1].Indices[0]);
            Assert.AreEqual(2, target.Classes[1].Indices[1]);
            Assert.AreEqual(0, target.CumulativeProportions.Length);
            Assert.AreEqual(0, target.DiscriminantMatrix.GetLength(0)); // dimension
            Assert.AreEqual(0, target.DiscriminantMatrix.GetLength(1)); // components kept
            Assert.AreEqual(0, target.DiscriminantProportions.Length);
            Assert.AreEqual(0, target.Discriminants.Count);
        }

#if !NO_BINARY_SERIALIZATION
        [Test]
        [Category("Serialization")]
        public void SerializeTest()
        {
            double[][] actual, expected = new double[][] {
                new double[] { -109.160894622401, -127.729010764102 },
                new double[] { -109.194678442625, -114.24653758324 },
                new double[] { -109.238116380388, -112.905892408598 },
                new double[] { -109.209886124532, -132.26101651421 },
                new double[] { -109.174352521775, -143.574080034334 },
                new double[] { -109.204229997471, -972.320404618979 },
                new double[] { 291.003271433059, 81.2380025750026 },
                new double[] { 290.982068268582, -259.413571936544 },
                new double[] { 290.973346814048, -161.838508509099 },
                new double[] { 290.998656827956, -728.677216732875 }
            };

            int[] output = { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 };

            var target = new KernelDiscriminantAnalysis()
            {
                Kernel = new Polynomial(4)
            };

            double[][] inputs = LinearDiscriminantAnalysisTest.inputs.ToJagged();
            int[] outputs = LinearDiscriminantAnalysisTest.output;
            target.Learn(inputs, output);

            actual = target.Transform(inputs);
            var str = actual.ToCSharp();
            Assert.IsTrue(Matrix.IsEqual(actual, expected, 0.01));

            var copy = Serializer.DeepClone(target);

            actual = copy.Transform(inputs);
            Assert.IsTrue(Matrix.IsEqual(actual, expected, 0.01));

            Assert.IsTrue(target.Kernel.Equals(copy.Kernel));
            Assert.IsTrue(target.ScatterBetweenClass.IsEqual(copy.ScatterBetweenClass));
            Assert.IsTrue(target.ScatterMatrix.IsEqual(copy.ScatterMatrix));
            Assert.IsTrue(target.ScatterWithinClass.IsEqual(copy.ScatterWithinClass));
            Assert.IsTrue(target.StandardDeviations.IsEqual(copy.StandardDeviations));
            Assert.IsTrue(target.Classifications.IsEqual(copy.Classifications));
            Assert.IsTrue(target.Classifier.NumberOfInputs.IsEqual(copy.Classifier.NumberOfInputs));
            Assert.IsTrue(target.Classifier.NumberOfOutputs.IsEqual(copy.Classifier.NumberOfOutputs));
            Assert.IsTrue(target.Classifier.First.Weights.IsEqual(copy.Classifier.First.Weights));
            Assert.IsTrue(target.Classifier.Second.Function.Equals(copy.Classifier.Second.Function));
            Assert.IsTrue(target.Classifier.Second.Means.IsEqual(copy.Classifier.Second.Means));
            Assert.IsTrue(target.NumberOfClasses.IsEqual(copy.NumberOfClasses));
            Assert.IsTrue(target.NumberOfInputs.Equals(copy.NumberOfInputs));
            Assert.IsTrue(target.NumberOfOutputs.Equals(copy.NumberOfOutputs));
        }
#endif
    }
}
