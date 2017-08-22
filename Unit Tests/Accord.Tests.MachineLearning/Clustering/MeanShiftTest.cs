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

namespace Accord.Tests.MachineLearning
{
    using Accord.MachineLearning;
    using Accord.Math;
    using Accord.Statistics.Distributions.DensityKernels;
    using NUnit.Framework;

    [TestFixture]
    public class MeanShiftTest
    {

        [Test]
        public void MeanShiftConstructorTest()
        {
            Accord.Math.Random.Generator.Seed = 0;

            // Test Samples
            double[][] samples =
            {
                new double[] { 0, 1 },
                new double[] { 1, 2 },
                new double[] { 1, 1 },
                new double[] { 0, 7 },
                new double[] { 1, 1 },
                new double[] { 6, 2 },
                new double[] { 6, 5 },
                new double[] { 5, 1 },
                new double[] { 7, 1 },
                new double[] { 5, 1 }
            };


            var kernel = new GaussianKernel(dimension: 2);
            MeanShift meanShift = new MeanShift(2, kernel, 2.0);
            meanShift.UseParallelProcessing = false;

            // Compute the model (estimate)
            int[] labels = meanShift.Compute(samples);

            int a = labels[0];
            int b = (a == 0) ? 1 : 0;

            for (int i = 0; i < 5; i++)
                Assert.AreEqual(a, labels[i]);

            for (int i = 5; i < samples.Length; i++)
                Assert.AreEqual(b, labels[i]);

            Assert.AreEqual(1.1922811512028066, meanShift.Clusters.Modes[a][0], 1e-3);
            Assert.AreEqual(1.2567196159235963, meanShift.Clusters.Modes[a][1], 1e-3);

            Assert.AreEqual(5.2696337859175868, meanShift.Clusters.Modes[b][0], 1e-3);
            Assert.AreEqual(1.4380326532534968, meanShift.Clusters.Modes[b][1], 1e-3);

            Assert.AreEqual(2, meanShift.Clusters.Count);
            Assert.AreEqual(2, meanShift.Clusters.Modes.Length);

            Assert.AreEqual(0.5, meanShift.Clusters.Proportions[0]);
            Assert.AreEqual(0.5, meanShift.Clusters.Proportions[1]);
        }

        [Test]
        public void MeanShiftConstructorTest2()
        {
            Accord.Math.Tools.SetupGenerator(1);

            // Declare some observations
            double[][] observations =
            {
                new double[] { -5, -2, -4 },
                new double[] { -5, -5, -6 },
                new double[] {  2,  1,  1 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  2,  2 },
                new double[] {  3,  1,  2 },
                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            double[][] orig = observations.MemberwiseClone();

            // Create a uniform kernel density function
            UniformKernel kernel = new UniformKernel();

            // Create a new Mean-Shift algorithm for 3 dimensional samples
            MeanShift meanShift = new MeanShift(dimension: 3, kernel: kernel, bandwidth: 2);

            // Compute the algorithm, retrieving an integer array
            //  containing the labels for each of the observations
            int[] labels = meanShift.Compute(observations);

            // As a result, the first two observations should belong to the
            //  same cluster (thus having the same label). The same should
            //  happen to the next four observations and to the last three.

            Assert.AreEqual(labels[0], labels[1]);

            Assert.AreEqual(labels[2], labels[3]);
            Assert.AreEqual(labels[2], labels[4]);
            Assert.AreEqual(labels[2], labels[5]);

            Assert.AreEqual(labels[6], labels[7]);
            Assert.AreEqual(labels[6], labels[8]);

            Assert.AreNotEqual(labels[0], labels[2]);
            Assert.AreNotEqual(labels[2], labels[6]);
            Assert.AreNotEqual(labels[0], labels[6]);


            int[] labels2 = meanShift.Clusters.Decide(observations);
            Assert.IsTrue(labels.IsEqual(labels2));

            // the data must not have changed!
            Assert.IsTrue(orig.IsEqual(observations));

            Assert.AreEqual(3 / 9.0, meanShift.Clusters.Proportions[labels[6]], 1e-6);
            Assert.AreEqual(2 / 9.0, meanShift.Clusters.Proportions[labels[0]], 1e-6);
            Assert.AreEqual(4 / 9.0, meanShift.Clusters.Proportions[labels[2]], 1e-6);
        }

        [Test]
        public void meanshift_new_method()
        {
            // Use a fixed seed for reproducibility
            Accord.Math.Random.Generator.Seed = 0;

            // Declare some data to be clustered
            double[][] input =
            {
                new double[] { -5, -2, -4 },
                new double[] { -5, -5, -6 },
                new double[] {  2,  1,  1 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  2,  2 },
                new double[] {  3,  1,  2 },
                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            // Create a uniform kernel density function
            UniformKernel kernel = new UniformKernel();

            // Create a new Mean-Shift algorithm for 3 dimensional samples
            MeanShift meanShift = new MeanShift(dimension: 3, kernel: kernel, bandwidth: 2);

            // Learn a data partitioning using the Mean Shift algorithm
            MeanShiftClusterCollection clustering = meanShift.Learn(input);

            // Predict group labels for each point
            int[] labels = clustering.Decide(input);

            // As a result, the first two observations should belong to the
            //  same cluster (thus having the same label). The same should
            //  happen to the next four observations and to the last three.

            Assert.AreEqual(labels[0], labels[1]);

            Assert.AreEqual(labels[2], labels[3]);
            Assert.AreEqual(labels[2], labels[4]);
            Assert.AreEqual(labels[2], labels[5]);

            Assert.AreEqual(labels[6], labels[7]);
            Assert.AreEqual(labels[6], labels[8]);

            Assert.AreNotEqual(labels[0], labels[2]);
            Assert.AreNotEqual(labels[2], labels[6]);
            Assert.AreNotEqual(labels[0], labels[6]);


            int[] labels2 = meanShift.Clusters.Decide(input);
            Assert.IsTrue(labels.IsEqual(labels2));

            Assert.AreEqual(3 / 9.0, meanShift.Clusters.Proportions[labels[6]], 1e-6);
            Assert.AreEqual(2 / 9.0, meanShift.Clusters.Proportions[labels[0]], 1e-6);
            Assert.AreEqual(4 / 9.0, meanShift.Clusters.Proportions[labels[2]], 1e-6);
        }


        [Test]
        public void meanshift_new_method_no_ctor_args()
        {
            #region doc_sample1
            // Use a fixed seed for reproducibility
            Accord.Math.Random.Generator.Seed = 0;

            // Declare some data to be clustered
            double[][] input =
            {
                new double[] { -5, -2, -4 },
                new double[] { -5, -5, -6 },
                new double[] {  2,  1,  1 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  2,  2 },
                new double[] {  3,  1,  2 },
                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            // Create a new Mean-Shift algorithm for 3 dimensional samples
            MeanShift meanShift = new MeanShift()
            {
                // Use a uniform kernel density
                Kernel = new UniformKernel(),
                Bandwidth = 2.0
            };

            // Learn a data partitioning using the Mean Shift algorithm
            MeanShiftClusterCollection clustering = meanShift.Learn(input);

            // Predict group labels for each point
            int[] labels = clustering.Decide(input);

            // As a result, the first two observations should belong to the
            //  same cluster (thus having the same label). The same should
            //  happen to the next four observations and to the last three.
            #endregion

            Assert.AreEqual(labels[0], labels[1]);

            Assert.AreEqual(labels[2], labels[3]);
            Assert.AreEqual(labels[2], labels[4]);
            Assert.AreEqual(labels[2], labels[5]);

            Assert.AreEqual(labels[6], labels[7]);
            Assert.AreEqual(labels[6], labels[8]);

            Assert.AreNotEqual(labels[0], labels[2]);
            Assert.AreNotEqual(labels[2], labels[6]);
            Assert.AreNotEqual(labels[0], labels[6]);


            int[] labels2 = meanShift.Clusters.Decide(input);
            Assert.IsTrue(labels.IsEqual(labels2));

            Assert.AreEqual(3 / 9.0, meanShift.Clusters.Proportions[labels[6]], 1e-6);
            Assert.AreEqual(2 / 9.0, meanShift.Clusters.Proportions[labels[0]], 1e-6);
            Assert.AreEqual(4 / 9.0, meanShift.Clusters.Proportions[labels[2]], 1e-6);
        }


        [Test]
        public void WeightedMeanShiftConstructorTest()
        {
            MeanShift ms1, ms2, ms3;

            Accord.Math.Tools.SetupGenerator(0);

            double[][] samples1 =
            {
                new double[] { 0, 1 },
                new double[] { 1, 2 },
                new double[] { 1, 1 },
                new double[] { 0, 1 },
                new double[] { 1, 1 },
                new double[] { 6, 2 },
                new double[] { 6, 5 },
                new double[] { 5, 1 },
                new double[] { 7, 1 },
                new double[] { 5, 1 }
            };

            int[] weights1 = Vector.Ones<int>(samples1.Length);

            ms1 = new MeanShift(2, new GaussianKernel(dimension: 2), 2.0);
            ms1.UseParallelProcessing = false;
            ms1.Compute(samples1);


            Accord.Math.Tools.SetupGenerator(0);

            double[][] samples2 =
            {
                new double[] { 0, 1 },
                new double[] { 1, 2 },
                new double[] { 1, 1 },
                new double[] { 0, 1 },
                new double[] { 6, 2 },
                new double[] { 6, 5 },
                new double[] { 5, 1 },
                new double[] { 7, 1 },
            };

            int[] weights = { 1, 1, 2, 1, 1, 1, 2, 1 };

            ms2 = new MeanShift(2, new GaussianKernel(dimension: 2), 2.0);
            ms2.UseParallelProcessing = false;
            ms2.Compute(samples2, weights);

            ms3 = new MeanShift(2, new GaussianKernel(dimension: 2), 2.0);
            ms3.UseParallelProcessing = false;
            ms3.Compute(samples2);


            int[] labels1 = ms1.Clusters.Decide(samples1);
            int[] labels2 = ms2.Clusters.Decide(samples1);
            int[] labels3 = ms3.Clusters.Decide(samples1);

            Assert.IsTrue(Matrix.IsEqual(labels1, labels2));

            Assert.IsTrue(Matrix.IsEqual(ms1.Clusters.Modes, ms2.Clusters.Modes, 1e-3));
            Assert.IsFalse(Matrix.IsEqual(ms1.Clusters.Modes, ms3.Clusters.Modes, 1e-2));
            Assert.IsFalse(Matrix.IsEqual(ms2.Clusters.Modes, ms3.Clusters.Modes, 1e-2));

            Assert.IsTrue(Matrix.IsEqual(ms1.Clusters.Proportions, ms2.Clusters.Proportions));
            Assert.IsTrue(Matrix.IsEqual(ms1.Clusters.Proportions, ms3.Clusters.Proportions));

            Assert.AreEqual(0.5, ms1.Clusters.Proportions[0]);
            Assert.AreEqual(0.5, ms1.Clusters.Proportions[1]);
        }

        [Test]
        public void WeightedMeanShiftConstructorTest2()
        {
            Accord.Math.Tools.SetupGenerator(1);

            // Declare some observations
            double[][] observations1 =
            {
                new double[] { -5, -2, -4 },
                new double[] { -5, -5, -6 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  1,  2 },
                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            int[] weights1 = { 1, 1, 1, 1, 1, 1, 1, 1, 1 };

            // Declare some observations
            double[][] observations2 =
            {
                new double[] { -5, -2, -4 },
                new double[] { -5, -5, -6 },
                new double[] {  1,  1,  2 },
                // new double[] {  1,  1,  2 },
                // new double[] {  1,  1,  2 },
                // new double[] {  1,  1,  2 },
                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            int[] weights2 = { 1, 1, 4, 1, 1, 1 };


            Accord.Math.Random.Generator.Seed = 1;
            var ms1 = new MeanShift(dimension: 3, kernel: new UniformKernel(), bandwidth: 2);
            ms1.UseParallelProcessing = false;
            ms1.Compute(observations1);

            Accord.Math.Random.Generator.Seed = 1;
            var ms2 = new MeanShift(dimension: 3, kernel: new UniformKernel(), bandwidth: 2);
            ms2.UseParallelProcessing = false;
            ms2.Compute(observations1, weights1);

            Accord.Math.Random.Generator.Seed = 1;
            var ms3 = new MeanShift(dimension: 3, kernel: new UniformKernel(), bandwidth: 2);
            ms3.UseParallelProcessing = false;
            ms3.Compute(observations2, weights2);

            int[] labels1 = ms1.Clusters.Decide(observations1);
            int[] labels2 = ms2.Clusters.Decide(observations1);
            int[] labels3 = ms3.Clusters.Decide(observations1);

            Assert.IsTrue(Matrix.IsEqual(labels1, labels2));
            Assert.IsTrue(Matrix.IsEqual(labels1, labels3));

            Assert.IsTrue(Matrix.IsEqual(ms1.Clusters.Modes, ms2.Clusters.Modes, 1e-3));
            Assert.IsTrue(Matrix.IsEqual(ms1.Clusters.Modes, ms3.Clusters.Modes, 1e-3));

            Assert.IsTrue(Matrix.IsEqual(ms1.Clusters.Proportions, ms2.Clusters.Proportions));
            Assert.IsTrue(Matrix.IsEqual(ms1.Clusters.Proportions, ms3.Clusters.Proportions));

            Assert.AreEqual(3 / 9.0, ms3.Clusters.Proportions[labels1[6]], 1e-6);
            Assert.AreEqual(2 / 9.0, ms3.Clusters.Proportions[labels1[0]], 1e-6);
            Assert.AreEqual(4 / 9.0, ms3.Clusters.Proportions[labels1[2]], 1e-6);
        }


        [Test]
        public void YinYangMeanShiftTest()
        {
            Accord.Math.Random.Generator.Seed = 1;
            double[][] inputs = yinyang.Submatrix(null, 0, 1).ToJagged();
            int[] outputs = yinyang.GetColumn(2).ToInt32();

            MeanShift ms = new MeanShift(2, new GaussianKernel(dimension: 2), 0.55);

            int[] labels = ms.Compute(inputs);
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
