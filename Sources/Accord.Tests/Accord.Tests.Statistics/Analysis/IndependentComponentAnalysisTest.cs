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

namespace Accord.Tests.Statistics
{
    using Accord.Math;
    using Accord.Statistics.Analysis;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    [TestClass()]
    public class IndependentComponentAnalysisTest
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
        public void ComputeTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            // Let's create a random dataset containing
            // 5000 samples of two dimensional samples.
            //
            double[,] source = Matrix.Random(5000, 2);

            // Now, we will mix the samples the dimensions of the samples.
            // A small amount of the second column will be applied to the
            // first, and vice-versa. 
            //
            double[,] mix =
            {
                {  0.25, 0.25 },
                { -0.25, 0.75 },    
            };

            // mix the source data
            double[,] input = source.Multiply(mix);

            // Now, we can use ICA to identify any linear mixing between the variables, such
            // as the matrix multiplication we did above. After it has identified it, we will
            // be able to revert the process, retrieving our original samples again
            
            // Create a new Independent Component Analysis
            var ica = new IndependentComponentAnalysis(input);

            Assert.AreEqual(IndependentComponentAlgorithm.Parallel, ica.Algorithm);

            // Compute it 
            ica.Compute();

            // Now, we can retrieve the mixing and demixing matrices that were 
            // used to alter the data. Note that the analysis was able to detect
            // this information automatically:

            double[,] mixingMatrix = ica.MixingMatrix; // same as the 'mix' matrix
            double[,] revertMatrix = ica.DemixingMatrix; // inverse of the 'mix' matrix

            double[,] result = ica.Result;

            // Verify mixing matrix
            mixingMatrix = mixingMatrix.Divide(mixingMatrix.Sum().Sum());
            Assert.IsTrue(mix.IsEqual(mixingMatrix, 0.008));


            // Verify demixing matrix
            double[,] expected =
            {
                { 0.75, -0.25 },        
                { 0.25,  0.25 },
            };

            revertMatrix = revertMatrix.Divide(revertMatrix.Sum().Sum());
            Assert.IsTrue(expected.IsEqual(revertMatrix, 0.008));
        }

        [TestMethod()]
        public void ComputeTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[,] S = Matrix.Random(5000, 2);

            double[,] A =
            {
                {  1, 1 },
                { -1, 3 },    
            };

            A = A.Divide(Norm.Norm1(A));

            double[,] X = S.Multiply(A);

            IndependentComponentAnalysis ica = new IndependentComponentAnalysis(X, IndependentComponentAlgorithm.Deflation);

            Assert.AreEqual(IndependentComponentAlgorithm.Deflation, ica.Algorithm);

            ica.Compute(2);

            var result = ica.Result;
            var mixingMatrix = ica.MixingMatrix;
            var revertMatrix = ica.DemixingMatrix;

            // Verify mixing matrix
            mixingMatrix = mixingMatrix.Divide(Norm.Norm1(mixingMatrix));
            Assert.IsTrue(A.IsEqual(mixingMatrix, 0.05));


            // Verify demixing matrix
            double[,] expected =
            {
                { 3, -1 },        
                { 1,  1 },
            };

            expected = expected.Divide(Norm.Norm1(expected));

            revertMatrix = revertMatrix.Divide(Norm.Norm1(revertMatrix));
            Assert.IsTrue(expected.IsEqual(revertMatrix, 0.05));



            var reverted = Accord.Statistics.Tools.ZScores(result).Abs();
            var original = Accord.Statistics.Tools.ZScores(S).Abs();

            Assert.IsTrue(reverted.IsEqual(original, 0.1));
        }

        [TestMethod()]
        public void SeparateTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[,] S = Matrix.Random(5000, 2);

            double[,] A =
            {
                {  0.25, 0.25 },
                { -0.25, 0.75 },    
            };

            double[,] X = S.Multiply(A);

            IndependentComponentAnalysis ica = new IndependentComponentAnalysis(X);


            ica.Compute(2);

            var expected = ica.Result;
            var actual = ica.Separate(X);

            Assert.IsTrue(expected.IsEqual(actual, 1e-4));
        }

        [TestMethod()]
        public void CombineTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[,] S = Matrix.Random(5000, 2);

            double[,] A =
            {
                {  0.25, 0.25 },
                { -0.25, 0.75 },    
            };

            double[,] X = S.Multiply(A);

            IndependentComponentAnalysis ica = new IndependentComponentAnalysis(X);


            ica.Compute(2);

            var result = ica.Result;

            var expected = Accord.Statistics.Tools.ZScores(X);
            var actual = Accord.Statistics.Tools.ZScores(ica.Combine(result));

            Assert.IsTrue(expected.IsEqual(actual, 1e-4));
        }

        [TestMethod()]
        public void CombineTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[,] S = Matrix.Random(5000, 2);

            double[,] A =
            {
                {  0.25, 0.25 },
                { -0.25, 0.75 },    
            };

            double[,] X = S.Multiply(A);

            IndependentComponentAnalysis ica = new IndependentComponentAnalysis(X);

            ica.Compute(2);

            double[,] result = ica.Result;


            float[][] expected = ica.Combine(result).ToSingle().ToArray(true);
            float[][] actual = ica.Combine(result.ToSingle().ToArray(true));

            Assert.IsTrue(expected.IsEqual(actual, 1e-4f));
        }

        [TestMethod()]
        public void SeparateTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[,] S = Matrix.Random(5000, 2);

            double[,] A =
            {
                {  0.25, 0.25 },
                { -0.25, 0.75 },    
            };

            double[,] X = S.Multiply(A);

            IndependentComponentAnalysis ica = new IndependentComponentAnalysis(X);


            ica.Compute(2);

            var expected = ica.Result.ToSingle().ToArray(true);
            var actual = ica.Separate(X.ToSingle().ToArray(true));

            Assert.IsTrue(expected.IsEqual(actual, 1e-4f));
        }

        [TestMethod()]
        public void SerializeTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[,] source = Matrix.Random(5000, 2);

            double[,] mix =
            {
                {  0.25, 0.25 },
                { -0.25, 0.75 },    
            };

            double[,] input = source.Multiply(mix);

            var ica = new IndependentComponentAnalysis(input);

            ica.Compute();

            MemoryStream stream = new MemoryStream();

            {
                BinaryFormatter b = new BinaryFormatter();
                b.Serialize(stream, ica);
            }

            stream.Seek(0, SeekOrigin.Begin);

            {
                BinaryFormatter b = new BinaryFormatter();
                ica = (IndependentComponentAnalysis)b.Deserialize(stream);
            }


            Assert.AreEqual(IndependentComponentAlgorithm.Parallel, ica.Algorithm);

            double[,] mixingMatrix = ica.MixingMatrix; // same as the 'mix' matrix
            double[,] revertMatrix = ica.DemixingMatrix; // inverse of the 'mix' matrix

            double[,] result = ica.Result;

            mixingMatrix = mixingMatrix.Divide(mixingMatrix.Sum().Sum());
            Assert.IsTrue(mix.IsEqual(mixingMatrix, 0.008));

            double[,] expected =
            {
                { 0.75, -0.25 },        
                { 0.25,  0.25 },
            };

            revertMatrix = revertMatrix.Divide(revertMatrix.Sum().Sum());
            Assert.IsTrue(expected.IsEqual(revertMatrix, 0.008));


        }

    }
}
