// Accord Unit Tests
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
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

using Accord.Statistics.Kernels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
namespace Accord.Tests.MachineLearning
{


    /// <summary>
    ///This is a test class for DynamicalTimeWarpingTest and is intended
    ///to contain all DynamicalTimeWarpingTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DynamicalTimeWarpingTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for DynamicalTimeWarping Constructor
        ///</summary>
        [TestMethod()]
        public void DynamicalTimeWarpingConstructorTest()
        {
            double[][] sequences = 
            {
                new double[] // -1
                {
                    0, 0, 0,
                    1, 1, 1,
                    2, 2, 2,
                },

                new double[] // -1
                {
                     0, 1, 0,
                     0, 2, 0,
                     0, 3, 0
                },

                new double[] // +1
                {
                     1, 1, 0,
                     1, 2, 0,
                     2, 1, 0,
                },

                new double[] // +1
                {
                     0, 0, 1, 
                     0, 0, 2, 
                     0, 1, 3,
                },
            };

            int[] outputs = { -1, -1, +1, +1 };


            // Set the parameters of the kernel
            double alpha = 0.85;
            int innerVectorLength = 3;


            // Create the kernel. Note that the input vector will be given out automatically
            DynamicTimeWarping target = new DynamicTimeWarping(innerVectorLength, alpha);



            // When using variable-length kernels, specify 0 as the input length.
            KernelSupportVectorMachine svm = new KernelSupportVectorMachine(target, 0);

            // Create the Sequential Minimal Optimization as usual
            SequentialMinimalOptimization smo = new SequentialMinimalOptimization(svm, sequences, outputs);
            smo.Complexity = 1.5;
            double error = smo.Run();


            // Computing the training values
            var a0 = svm.Compute(sequences[0]);
            var a1 = svm.Compute(sequences[1]);
            var a2 = svm.Compute(sequences[2]);
            var a3 = svm.Compute(sequences[3]);

            Assert.AreEqual(-1, System.Math.Sign(a0));
            Assert.AreEqual(-1, System.Math.Sign(a1));
            Assert.AreEqual(+1, System.Math.Sign(a2));
            Assert.AreEqual(+1, System.Math.Sign(a3));



            // Computing a new testing value
            double[] test =
                {
                     1, 0, 1,
                     0, 0, 2,
                     0, 1, 3,
                };

            var a4 = svm.Compute(test);

        }

        /// <summary>
        ///A test for DynamicalTimeWarping Constructor
        ///</summary>
        [TestMethod()]
        public void DynamicalTimeWarpingConstructorTest2()
        {
            // Declare some testing data
            double[][] inputs =
            {
                // Class -1
                new double[] { 0,1,1,0 },
                new double[] { 0,0,1,0 },  
                new double[] { 0,1,1,1,0 }, 
                new double[] { 0,1,0 },    

                // Class +1
                new double[] { 1,0,0,1 },   
                new double[] { 1,1,0,1 }, 
                new double[] { 1,0,0,0,1 },
                new double[] { 1,0,1 },   
                new double[] { 1,0,0,0,1,1 } 
            };

            int[] outputs =
            {
                -1,-1,-1,-1,  // First four sequences are of class -1
                 1, 1, 1, 1, 1 // Last five sequences are of class +1
            };


            // Set the parameters of the kernel
            double alpha = 1.0;
            int degree = 1;
            int innerVectorLength = 1;

            // Create the kernel. Note that the input vector will be given out automatically
            DynamicTimeWarping target = new DynamicTimeWarping(innerVectorLength, alpha, degree);


            // When using variable-length kernels, specify 0 as the input length.
            KernelSupportVectorMachine svm = new KernelSupportVectorMachine(target, 0);

            // Create the Sequential Minimal Optimization as usual
            SequentialMinimalOptimization smo = new SequentialMinimalOptimization(svm, inputs, outputs);
            smo.Complexity = 1.5;
            double error = smo.Run();


            // Check if the model has learnt the sequences correctly.
            for (int i = 0; i < inputs.Length; i++)
            {
                int expected = outputs[i];
                int actual = System.Math.Sign(svm.Compute(inputs[i]));
                Assert.AreEqual(expected, actual);
            }

            // Testing new sequences
            Assert.AreEqual(-1,System.Math.Sign(svm.Compute(new double[] { 0, 1, 1, 0, 0 })));
            Assert.AreEqual(+1,System.Math.Sign(svm.Compute(new double[] { 1, 1, 0, 0, 1, 1 })));
        }
    }
}
