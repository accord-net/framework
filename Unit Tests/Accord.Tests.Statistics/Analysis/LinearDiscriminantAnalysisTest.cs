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
    using Accord.Math;
    using IO;

    [TestFixture]
    public class LinearDiscriminantAnalysisTest
    {

        // This is the same data used in the example by Gutierrez-Osuna
        // http://research.cs.tamu.edu/prism/lectures/pr/pr_l10.pdf

        public static double[,] inputs =
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

        public static int[] output = 
        {
            1, 1, 1, 1, 1, // Class labels for the input vectors
            2, 2, 2, 2, 2
        };


        [Test]
        public void ComputeTest()
        {
            LinearDiscriminantAnalysis lda = new LinearDiscriminantAnalysis(inputs, output);

            // Compute the analysis
            lda.Compute();

            double[,] expectedScatter1 = 
            {
                {  0.80, -0.40 }, 
                { -0.40,  2.64 } 
            };

            double[,] expectedScatter2 = 
            {
                {  1.84, -0.04 }, 
                { -0.04,  2.64 }
            };

            double[,] expectedBetween = 
            {
                { 29.16, 21.60 },
                { 21.60, 16.00 },
            };

            double[,] expectedWithin = 
            {
                {  2.64, -0.44 },
                { -0.44,  5.28 }
            };

            Assert.IsTrue(Matrix.IsEqual(lda.Classes[0].Scatter, expectedScatter1, 0.01));
            Assert.IsTrue(Matrix.IsEqual(lda.Classes[1].Scatter, expectedScatter2, 0.01));

            Assert.IsTrue(Matrix.IsEqual(lda.ScatterBetweenClass, expectedBetween, 0.01));
            Assert.IsTrue(Matrix.IsEqual(lda.ScatterWithinClass, expectedWithin, 0.01));
        }

        [Test]
        public void ClassifyTest1()
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

            // Then, we will create a LDA for the given instances.
            var lda = new LinearDiscriminantAnalysis(inputs, output);

            lda.Compute(); // Compute the analysis


            // Now we can project the data into KDA space:
            double[][] projection = lda.Transform(inputs);

            double[][] classifierProjection = lda.Classifier.First.Transform(inputs);
            Assert.IsTrue(projection.IsEqual(classifierProjection));

            // Or perform classification using:
            int[] results = lda.Classify(inputs);

            double[][] expected = new double[][] 
            {
                new double[] { 4.42732558139535, 1.96296296296296 },
                new double[] { 3.7093023255814, -2.51851851851852 },
                new double[] { 3.28197674418605, -1.51851851851852 },
                new double[] { 5.56395348837209, -3.77777777777778 },
                new double[] { 5.7093023255814, -1.03703703703704 },
                new double[] { 13.2732558139535, -3.33333333333333 },
                new double[] { 9.41860465116279, -3.55555555555556 },
                new double[] { 11.1366279069767, 1.66666666666667 },
                new double[] { 10.9912790697674, -1.07407407407407 },
                new double[] { 13.4186046511628, -0.592592592592593 } 
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
        public void new_method()
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

            // We will create a LDA object for the data
            var lda = new LinearDiscriminantAnalysis();

            // Compute the analysis and create a classifier
            var classifier = lda.Learn(inputs, output);

            // Now we can project the data into LDA space:
            double[][] projection = lda.Transform(inputs);

            // Or perform classification using:
            int[] results = classifier.Decide(inputs);
            #endregion

            double[][] classifierProjection = classifier.First.Transform(inputs);
            Assert.IsTrue(projection.IsEqual(classifierProjection));

            double[][] expected = new double[][] 
            {
                new double[] { 4.42732558139535, 1.96296296296296 },
                new double[] { 3.7093023255814, -2.51851851851852 },
                new double[] { 3.28197674418605, -1.51851851851852 },
                new double[] { 5.56395348837209, -3.77777777777778 },
                new double[] { 5.7093023255814, -1.03703703703704 },
                new double[] { 13.2732558139535, -3.33333333333333 },
                new double[] { 9.41860465116279, -3.55555555555556 },
                new double[] { 11.1366279069767, 1.66666666666667 },
                new double[] { 10.9912790697674, -1.07407407407407 },
                new double[] { 13.4186046511628, -0.592592592592593 } 
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
        public void ComputeTest2()
        {
            LinearDiscriminantAnalysis lda = new LinearDiscriminantAnalysis(inputs, output);

            // Compute the analysis
            lda.Compute();

            Assert.AreEqual(2, lda.Classes.Count);
            Assert.AreEqual(3.0, lda.Classes[0].Mean[0]);
            Assert.AreEqual(3.6, lda.Classes[0].Mean[1]);
            Assert.AreEqual(5, lda.Classes[0].Indices.Length);

            Assert.AreEqual(0, lda.Classes[0].Indices[0]);
            Assert.AreEqual(1, lda.Classes[0].Indices[1]);
            Assert.AreEqual(2, lda.Classes[0].Indices[2]);
            Assert.AreEqual(3, lda.Classes[0].Indices[3]);
            Assert.AreEqual(4, lda.Classes[0].Indices[4]);

            Assert.AreEqual(5, lda.Classes[1].Indices[0]);
            Assert.AreEqual(6, lda.Classes[1].Indices[1]);
            Assert.AreEqual(7, lda.Classes[1].Indices[2]);
            Assert.AreEqual(8, lda.Classes[1].Indices[3]);
            Assert.AreEqual(9, lda.Classes[1].Indices[4]);

            Assert.AreEqual(2, lda.Discriminants.Count);
            Assert.AreEqual(15.65685019206146, lda.Discriminants[0].Eigenvalue);
            Assert.AreEqual(-0.00000000000000, lda.Discriminants[1].Eigenvalue, 1e-15);

            Assert.AreEqual(5.7, lda.Means[0]);
            Assert.AreEqual(5.6, lda.Means[1]);
        }

        [Test]
        public void large_transform_few_components()
        {
            int n = 100;
            double[][] data = Jagged.Random(n, n);
            int[] labels = Vector.Random(n, 0, 10);

            var kda = new LinearDiscriminantAnalysis();
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
        public void ProjectionTest()
        {
            var lda = new LinearDiscriminantAnalysis(inputs, output);

            // Compute the analysis
            lda.Compute();

            // Project the input data into discriminant space
            double[,] projection = lda.Transform(inputs);

            double tol = 1e-15;
            Assert.AreEqual(projection[0, 0], 4.4273255813953485, tol);
            Assert.AreEqual(projection[0, 1], 1.9629629629629628, tol);
            Assert.AreEqual(projection[1, 0], 3.7093023255813953, tol);
            Assert.AreEqual(projection[1, 1], -2.5185185185185186, tol);
            Assert.AreEqual(projection[2, 0], 3.2819767441860463, tol);
            Assert.AreEqual(projection[2, 1], -1.5185185185185186, tol);
            Assert.AreEqual(projection[3, 0], 5.5639534883720927, tol);
            Assert.AreEqual(projection[3, 1], -3.7777777777777777, tol);
            Assert.AreEqual(projection[4, 0], 5.7093023255813957, tol);
            Assert.AreEqual(projection[4, 1], -1.0370370370370372, tol);
            Assert.AreEqual(projection[5, 0], 13.273255813953488, tol);
            Assert.AreEqual(projection[5, 1], -3.3333333333333339, tol);
            Assert.AreEqual(projection[6, 0], 9.4186046511627914, tol);
            Assert.AreEqual(projection[6, 1], -3.5555555555555554, tol);
            Assert.AreEqual(projection[7, 0], 11.136627906976745, tol);
            Assert.AreEqual(projection[7, 1], 1.6666666666666661, tol);
            Assert.AreEqual(projection[8, 0], 10.991279069767442, tol);
            Assert.AreEqual(projection[8, 1], -1.0740740740740744, tol);
            Assert.AreEqual(projection[9, 0], 13.418604651162791, tol);
            Assert.AreEqual(projection[9, 1], -0.59259259259259345, tol);

            // Assert the result equals the transformation of the input
            double[,] result = lda.Result;
            Assert.IsTrue(Matrix.IsEqual(result, projection));
            Assert.IsFalse(Matrix.HasNaN(projection));
        }

        [Test]
        public void ClassifyTest()
        {
            var lda = new LinearDiscriminantAnalysis(inputs, output);

            // Compute the analysis
            lda.Compute();

            for (int i = 0; i < output.Length; i++)
                Assert.AreEqual(output[i], lda.Classify(inputs.GetRow(i)));
        }

        [Test]
        public void ComputeTest3()
        {
            // Schölkopf KPCA toy example
            double[][] inputs = KernelDiscriminantAnalysisTest.scholkopf().ToJagged();

            int[] output = Matrix.Expand(new int[,] { { 1 }, { 2 }, { 3 } }, new int[] { 30, 30, 30 }).GetColumn(0);

            var target = new LinearDiscriminantAnalysis(inputs, output);

            target.Compute();


            double[][] actualOutput = target.Transform(inputs, 2);

            double[][] expectedOutput = new double[][] 
            {
                new double[] { -0.538139989229878, -0.121488441426448 },
                new double[] { -0.520567977909383, -0.236347775257103 },
                new double[] { -0.613477771536265, -0.237553378277353 },
                new double[] { -0.881409292261883, 0.0935329540430248 },
                new double[] { -0.786030327227691, -0.194447244320605 },
                new double[] { -0.551442781305912, -0.0123559223681317 },
                new double[] { -0.654158684224005, -0.197674316111905 },
                new double[] { -0.559262527603992, 0.013941098843778 },
                new double[] { -0.66411263033584, -0.150490536781379 },
                new double[] { -0.450278115670319, -0.26635277047329 },
                new double[] { -0.754277919814726, -0.362102431498804 },
                new double[] { -0.734928584895253, -0.248980106866025 },
                new double[] { -0.653608644698921, 0.0143647201181394 },
                new double[] { -0.760931829205159, -0.210515053383166 },
                new double[] { -0.618516474044195, -0.142285367330635 },
                new double[] { -0.779342671809792, -0.141199637690287 },
                new double[] { -0.735924645881001, -0.146617711795974 },
                new double[] { -0.785744941649802, -0.31168984794763 },
                new double[] { -0.669124608334209, -0.420106774148463 },
                new double[] { -0.824474062918818, 0.147088211780247 },
                new double[] { -0.799320425464541, -0.0637527478684568 },
                new double[] { -0.663385572908364, -0.341675337652223 },
                new double[] { -0.711489490612721, -0.285076461900782 },
                new double[] { -0.629974516987287, -0.0793021800418604 },
                new double[] { -0.65653220838978, -0.215831476310217 },
                new double[] { -0.732028761895192, -0.0344445204239324 },
                new double[] { -0.747862524505661, -0.0387281405057906 },
                new double[] { -0.584471308297719, -0.146019839184912 },
                new double[] { -0.505999843470041, -0.292203766994798 },
                new double[] { -0.753145346001892, -0.344521076589941 },
                new double[] { 0.524001176904191, -0.64158358593467 },
                new double[] { 0.423231837049123, -0.286159521674357 },
                new double[] { 0.656426922526874, -0.734236743185728 },
                new double[] { 0.400687334850924, -0.55115062988607 },
                new double[] { 0.636240473795815, -0.748303834209756 },
                new double[] { 0.434843292522556, -0.566740271085617 },
                new double[] { 0.6104713046872, -0.678967931597066 },
                new double[] { 0.705262787734728, -0.640414054245901 },
                new double[] { 0.447832238019099, -0.661180602320438 },
                new double[] { 0.659661046824861, -0.630212303468225 },
                new double[] { 0.672147865129419, -0.503357319234685 },
                new double[] { 0.638711507323203, -0.644310115155753 },
                new double[] { 0.536863923134139, -0.438197907521421 },
                new double[] { 0.496141960347812, -0.530750925839334 },
                new double[] { 0.906503239478175, -0.59100400335581 },
                new double[] { 0.604370405460113, -0.46954478102178 },
                new double[] { 0.412131895699799, -0.758049666960606 },
                new double[] { 0.423464497686766, -0.438725534434289 },
                new double[] { 0.351983120391112, -0.693723302359591 },
                new double[] { 0.600453835286623, -0.446793343407863 },
                new double[] { 0.585438337076168, -0.544511883828685 },
                new double[] { 0.727841528212698, -0.650301108602448 },
                new double[] { 0.751448391254333, -0.633046233976002 },
                new double[] { 0.857558106835016, -0.587237152739008 },
                new double[] { 0.554131023905099, -0.639630778761857 },
                new double[] { 0.604769997035484, -0.660127547060936 },
                new double[] { 0.532120384569746, -0.448864888884797 },
                new double[] { 0.62587117635701, -0.482512841662285 },
                new double[] { 0.580333409415421, -0.80962907380129 },
                new double[] { 0.601495554392805, -0.730598326012776 },
                new double[] { 0.593941507609862, 0.350118652741363 },
                new double[] { 0.357712432226073, 0.2963287302749 },
                new double[] { 0.551383385237947, 0.374412117881701 },
                new double[] { 0.690356212604399, 0.240090830766309 },
                new double[] { 0.462549608533101, 0.337029321214765 },
                new double[] { 0.613846624949793, 0.302978372516851 },
                new double[] { 0.632960280224768, 0.690169219132759 },
                new double[] { 0.56675518056767, 0.218090431387548 },
                new double[] { 0.511872653377024, 0.0692203349420495 },
                new double[] { 0.177443905363662, 0.23100145864499 },
                new double[] { 0.327851974844022, 0.415060901754569 },
                new double[] { 0.341124386412447, 0.416335789100053 },
                new double[] { 0.44860383164398, 0.214369753920447 },
                new double[] { 0.63110091195233, 0.59664872441043 },
                new double[] { 0.587620021924801, 0.451661866983025 },
                new double[] { 0.433140254056975, 0.56057876616672 },
                new double[] { 0.640109409731833, 0.298279362477078 },
                new double[] { 0.140413240631302, 0.233509735221199 },
                new double[] { 0.751771638050688, 0.407674765260726 },
                new double[] { 0.57522328805595, 0.296203994397562 },
                new double[] { 0.394007233177402, 0.32004606890218 },
                new double[] { 0.323309388831049, 0.188114883322704 },
                new double[] { 0.478221796731402, 0.409092441378802 },
                new double[] { 0.673650933463591, 0.561639241955278 },
                new double[] { 0.645748558652938, 0.282496300419708 },
                new double[] { 0.588553164739597, 0.428759787951118 },
                new double[] { 0.377052961673182, 0.466388880012159 },
                new double[] { 0.752164965657736, 0.289900686186869 },
                new double[] { 0.247467021467445, 0.361971115290112 },
                new double[] { 0.636721385361009, 0.399430035006511 } 
            };

            Assert.IsTrue(Matrix.IsEqual(actualOutput, expectedOutput, 1e-5));

            // Assert the result equals the transformation of the input
            double[][] result = target.Result.ToJagged();
            double[][] projection = target.Transform(inputs);
            Assert.IsTrue(Matrix.IsEqual(result, projection));
            Assert.IsTrue(Matrix.IsEqual(result, expectedOutput, 1e-6));


            int[] actual2 = target.Classify(inputs);
            Assert.IsTrue(Matrix.IsEqual(actual2, output));

            double[][] scores = new double[inputs.Length][];
            int[] actual3 = new int[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
                actual3[i] = target.Classify(inputs[i], out scores[i]);

            Assert.IsTrue(Matrix.IsEqual(actual3, output));
            var actualMeans = target.projectedMeans;

            scores = scores.Get(0, 5, null);
            //var str2 = scores.ToCSharp();

            var expected = new double[][] {
                new double[] { -0.0213345342185279, -1.48626837046456, -1.31720201011333 },
                new double[] { -0.0295574706116435, -1.35158673700292, -1.40393954892816 },
                new double[] { -0.0092314990039484, -1.5648696800027, -1.60459093003653 },
                new double[] { -0.108880936496527, -2.6251849668169, -1.99175104959092 },
                new double[] { -0.0126381832555252, -2.04103325730257, -1.97099575187989 } 
            };

            Assert.IsTrue(Matrix.IsEqual(scores, expected, 1e-6));
        }

#if !NO_BINARY_SERIALIZATION
        [Test]
        [Category("Serialization")]
        public void SerializeTest()
        {
            double[][] actual, expected = new double[][] {
                new double[] { 3.97283892300425, 1.19607843137255 },
                new double[] { 1.89135569201701, -2.90196078431373 },
                new double[] { 1.91851676901275, -1.90196078431373 },
                new double[] { 2.83703353802551, -4.35294117647059 },
                new double[] { 3.89135569201701, -1.80392156862745 },
                new double[] { 8.72838923004251, -5.05882352941177 },
                new double[] { 5.78271138403401, -4.70588235294118 },
                new double[] { 8.86419461502126, -0.0588235294117654 },
                new double[] { 7.80987246102976, -2.6078431372549 },
                new double[] { 9.78271138403401, -2.50980392156863 }
            };

            int[] output = { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 };

            var target = new LinearDiscriminantAnalysis();

            target.Learn(inputs.ToJagged(), output);

            actual = target.Transform(inputs.ToJagged());
            var str = actual.ToCSharp();
            Assert.IsTrue(Matrix.IsEqual(actual, expected, 0.01));

            var copy = Serializer.DeepClone(target);

            actual = copy.Transform(inputs.ToJagged());
            Assert.IsTrue(Matrix.IsEqual(actual, expected, 0.01));

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
