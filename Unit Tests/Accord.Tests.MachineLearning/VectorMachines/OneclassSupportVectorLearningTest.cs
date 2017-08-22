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
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Math;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Kernels;
    using Math.Optimization.Losses;
    using NUnit.Framework;
    using System.Linq;

    [TestFixture]
    public class OneclassSupportVectorLearningTest
    {

        [Test]
        public void RunTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            var dist = NormalDistribution.Standard;

            double[] x =
            {
                +1.0312479734420776,
                +0.99444115161895752,
                +0.21835240721702576,
                +0.47197291254997253,
                +0.68701112270355225,
                -0.58556461334228516,
                -0.64154046773910522,
                -0.66485315561294556,
                +0.37940266728401184,
                -0.61046308279037476
            };

            double[][] inputs = Jagged.ColumnVector(x);

            IKernel kernel = new Linear();

            var machine = new KernelSupportVectorMachine(kernel, inputs: 1);

            var teacher = new OneclassSupportVectorLearning(machine, inputs)
            {
                Nu = 0.1
            };

            // Run the learning algorithm
            double error = teacher.Run();

            Assert.AreEqual(2, machine.Weights.Length);
            Assert.AreEqual(0.39198910030993617, machine.Weights[0]);
            Assert.AreEqual(0.60801089969006383, machine.Weights[1]);
            Assert.AreEqual(inputs[0][0], machine.SupportVectors[0][0]);
            Assert.AreEqual(inputs[7][0], machine.SupportVectors[1][0]);
            Assert.AreEqual(0.0, error, 1e-10);
        }

        [Test]
        public void learn_test()
        {
            #region doc_learn
            // Ensure that results are reproducible
            Accord.Math.Random.Generator.Seed = 0;

            // Generate some data to be learned
            double[][] inputs =
            {
                new double[] { +1.0312479734420776  },
                new double[] { +0.99444115161895752 },
                new double[] { +0.21835240721702576 },
                new double[] { +0.47197291254997253 },
                new double[] { +0.68701112270355225 },
                new double[] { -0.58556461334228516 },
                new double[] { -0.64154046773910522 },
                new double[] { -0.66485315561294556 },
                new double[] { +0.37940266728401184 },
                new double[] { -0.61046308279037476 }
            };


            // Create a new One-class SVM learning algorithm
            var teacher = new OneclassSupportVectorLearning<Linear>()
            {
                Kernel = new Linear(), // or, for example, 'new Gaussian(0.9)'
                Nu = 0.1
            };

            // Learn a support vector machine
            var svm = teacher.Learn(inputs);

            // Test the machine
            double[] prediction = svm.Score(inputs);

            // Compute the log-likelihood of the answer
            double ll = new LogLikelihoodLoss().Loss(prediction);
            #endregion

            Assert.AreEqual(-1.6653345369377348E-16, ll, 1e-10);
            Assert.AreEqual(2, svm.Weights.Length);
            Assert.AreEqual(0.39198910030993617, svm.Weights[0], 1e-10);
            Assert.AreEqual(0.60801089969006383, svm.Weights[1], 1e-10);
            Assert.AreEqual(inputs[0][0], svm.SupportVectors[0][0], 1e-10);
            Assert.AreEqual(inputs[7][0], svm.SupportVectors[1][0], 1e-10);
        }


        [Test]
        public void square_test()
        {
            // Example from https://stackoverflow.com/questions/38642615/why-does-this-clean-data-provide-strange-svm-classification-results

            double[][] inputs = new double[49][];
            int i = 0;
            for (double x = -0.3; x <= 0.31; x += 0.1)
                for (double y = -0.3; y <= 0.31; y += 0.1)
                    inputs[i++] = new double[] { x, y };

            // Generate inlier and outlier test points.
            double[][] outliers =
            {
                new double[] { 1E6, 1E6 },  // Very far outlier
                new double[] { 0, 1E6 },    // Very far outlier
                new double[] { 100, -100 }, // Far outlier
                new double[] { 0, -100 },   // Far outlier
                new double[] { -10, -10 },  // Still far outlier
                new double[] { 0, -10 },    // Still far outlier
                new double[] { 0.6, 0.6 },  // Close outlier
                new double[] { 0.5, 0.0 }   // Close outlier
            };

            double[][] inliers =
            {
                new double[] { 0.0, 0.0 },   // Middle of cluster
                new double[] { .15, .15 },   // Halfway to corner of cluster
                new double[] { -0.1, 0 },    // Comfortably inside cluster
                new double[] { 0.25, 0 },    // Near inside edge of cluster
                new double[] { 0.28, 0.28 }  // Near inside edge of cluster
            };


            var teacher = new OneclassSupportVectorLearning<Gaussian>()
            {
                Nu = 0.05,
                Tolerance = 1e-2,
                Kernel = new Gaussian(1)
            };

            var svm = teacher.Learn(inputs);

            double[] a = svm.Score(outliers);
            double[] b = svm.Score(inliers);
            double[] c = svm.Score(inputs);

            string stra = a.ToCSharp();
            string strb = b.ToCSharp();
            string strc = c.ToCSharp();

            double[] ea =
            {
                -2.06303275869732, -2.06303275869732,
                -2.06303275869732, -2.06303275869732,
                -2.06303275869732, -2.06303275869732,
                -0.43909532904464, -0.0610610987108576
            };

            double[] eb =
            {
                0.176098645217194, 0.13254525498832,
                0.1651082775092, 0.115325884477755,
                0.0260693377780776
            };

            double[] ec =
            {
                -1.33226762955019E-15, 0.0467039286535355, 0.0749106707071278, 0.0838648156076344,
                0.0733262829494159, 0.0435809252346452, -0.00457227086652812, 0.0474811783155659,
                0.0955242612540407, 0.124652669315224, 0.134085905268251, 0.123570483126211,
                0.0933911526436562, 0.0443581748966757, 0.0764893734298348, 0.125461639137703,
                0.155267627012948, 0.165108277509199, 0.154718603422687, 0.12437945294869,
                0.0749049856721229, 0.086246524528469, 0.13571268664663, 0.165933589606133,
                0.176098645217194, 0.165933589606133, 0.135712686646629, 0.0862465245284685,
                0.0764893734298354, 0.126000305758801, 0.156361734225727, 0.166758901703067,
                0.156910757815989, 0.127082491947813, 0.0780737611875469, 0.0474811783155671,
                0.0965860327119062, 0.126809275581279, 0.137339468025008, 0.127891461770292,
                0.0987191413222903, 0.0506041817344567, 6.66133814775094E-16, 0.0482584279775976,
                0.0780680761525424, 0.0886282334493032, 0.0796524639102539, 0.051381431396487,
                0.00457227086652678
            };

            Assert.IsTrue(ea.IsEqual(a, 1e-5));
            Assert.IsTrue(eb.IsEqual(b, 1e-5));
            Assert.IsTrue(ec.IsEqual(c, 1e-5));

            bool[] da = svm.Decide(outliers);
            bool[] db = svm.Decide(inliers);

            Assert.IsTrue(da.All(x => x == false));
            Assert.IsTrue(db.All(x => x == true));
        }

    }
}
