// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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

    [TestFixture, Ignore]
    public class TSNETest
    {

        [Test]
        public void ConstructorTest()
        {
            Accord.Math.Random.Generator.Seed = 0;

            IdxReader idxReader = new IdxReader(Resources.train_images_idx3_ubyte);
            double[][] X = idxReader.ReadToEndAsVectors<double>();

            Assert.AreEqual(X.Length, 60000);
            Assert.AreEqual(X[59999].Length, 784);

            // Perform the initial dimensionality reduction using PCA
            //var pca = new PrincipalComponentAnalysis(numberOfOutputs: 2);
            //pca.Learn(X);
            //pca.Save(@"pca_v3_1.bin");

            var pca = Serializer.Load<PrincipalComponentAnalysis>(Properties.Resources.pca_mnist_v3_1);

            X = pca.Transform(X);

            TSNE tSNE = new TSNE();
            var Y = tSNE.Transform(X);

            Assert.Fail();
        }
    }
#endif
}
