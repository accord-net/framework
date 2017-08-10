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
#if !MONO
    using Accord.IO;
    using Accord.MachineLearning;
    using Accord.MachineLearning.Clustering;
    using Accord.Math;
    using Properties;
    using NUnit.Framework;
    using System.IO;
    using Accord.Statistics.Analysis;

    [TestFixture]
    public class TSNETest
    {

#if !NO_BINARY_SERIALIZATION
        [Test, Ignore("Test was not finished being written")]
        public void ConstructorTest()
        {
            Accord.Math.Random.Generator.Seed = 0;

            string mnistPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "mnist", "train-images-idx3-ubyte.gz");

            IdxReader idxReader = new IdxReader(mnistPath);
            double[][] X = idxReader.ReadToEndAsVectors<double>();

            Assert.AreEqual(X.Length, 60000);
            Assert.AreEqual(X[59999].Length, 784);

            // Perform the initial dimensionality reduction using PCA
            //var pca = new PrincipalComponentAnalysis(numberOfOutputs: 2);
            //pca.Learn(X);
            //pca.Save(@"pca_v3_1.bin");

            string pcaPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "mnist", "pca_mnist_v3_1.bin");

            var pca = Serializer.Load<PrincipalComponentAnalysis>(pcaPath);

            X = pca.Transform(X);

            TSNE tSNE = new TSNE();
            var Y = tSNE.Transform(X);

            Assert.Fail();
        }
#endif

        [Test, Ignore("Test was not finished being written")]
        public void learn_test()
        {
            #region doc_learn
            Accord.Math.Random.Generator.Seed = 0;

            // Declare some observations
            double[][] observations =
            {
                new double[] { -5, -2, -1 },
                new double[] { -5, -5, -6 },
                new double[] {  2,  1,  1 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  2,  2 },
                new double[] {  3,  1,  2 },
                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            // Create a new t-SNE algorithm 
            TSNE tSNE = new TSNE()
            {
                NumberOfOutputs = 1,
                Perplexity = 1.5
            };

            // Transform to a reduced dimensionality space
            double[][] output = tSNE.Transform(observations);

            // Make it 1-dimensional
            double[] y = output.Reshape();
            #endregion

            string str = y.ToCSharp();
            double[] expected = new double[] { 327.15556116089, 144.502680170483, -21.5116375004548, 253.712522074559, 214.067349874275, 24.8621254326599, -299.97879062709, -260.342898777221, -382.466911808102 };
            Assert.IsTrue(y.IsEqual(expected, rtol: 1e-5));
        }
    }
#endif
}
