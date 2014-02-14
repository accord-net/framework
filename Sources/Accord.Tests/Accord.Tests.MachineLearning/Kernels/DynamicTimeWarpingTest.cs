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
    using Accord.Statistics.Kernels;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Math;

    [TestClass()]
    public class DynamicalTimeWarpingTest
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

        [TestMethod()]
        public void DynamicalTimeWarpingConstructorTest3()
        {
            // Suppose you have sequences of multivariate observations, and that
            // those sequences could be of arbitrary length. On the other hand, 
            // each observation have a fixed, delimited number of dimensions.

            // In this example, we have sequences of 3-dimensional observations. 
            // Each sequence can have an arbitrary length, but each observation
            // will always have length 3:

            double[][][] sequences =
            {
                new double[][] // first sequence
                {
                    new double[] { 1, 1, 1 }, // first observation of the first sequence
                    new double[] { 1, 2, 1 }, // second observation of the first sequence
                    new double[] { 1, 4, 2 }, // third observation of the first sequence
                    new double[] { 2, 2, 2 }, // fourth observation of the first sequence
                },

                new double[][] // second sequence (note that this sequence has a different length)
                {
                    new double[] { 1, 1, 1 }, // first observation of the second sequence
                    new double[] { 1, 5, 6 }, // second observation of the second sequence
                    new double[] { 2, 7, 1 }, // third observation of the second sequence
                },

                new double[][] // third sequence 
                {
                    new double[] { 8, 2, 1 }, // first observation of the third sequence
                },

                new double[][] // fourth sequence 
                {
                    new double[] { 8, 2, 5 }, // first observation of the fourth sequence
                    new double[] { 1, 5, 4 }, // second observation of the fourth sequence
                }
            };

            // Now, we will also have different class labels associated which each 
            // sequence. We will assign -1 to sequences whose observations start 
            // with { 1, 1, 1 } and +1 to those that do not:

            int[] outputs =
            {
                -1,-1,  // First two sequences are of class -1 (those start with {1,1,1})
                    1, 1,  // Last two sequences are of class +1  (don't start with {1,1,1})
            };

            // At this point, we will have to "flat" out the input sequences from double[][][]
            // to a double[][] so they can be properly understood by the SVMs. The problem is 
            // that, normally, SVMs usually expect the data to be comprised of fixed-length 
            // input vectors and associated class labels. But in this case, we will be feeding
            // them arbitrary-length sequences of input vectors and class labels associated with
            // each sequence, instead of each vector.

            double[][] inputs = new double[sequences.Length][];
            for (int i = 0; i < sequences.Length; i++)
                inputs[i] = Matrix.Concatenate(sequences[i]);


            // Now we have to setup the Dynamic Time Warping kernel. We will have to
            // inform the length of the fixed-length observations contained in each
            // arbitrary-length sequence:
            // 
            DynamicTimeWarping kernel = new DynamicTimeWarping(length: 3);

            // Now we can create the machine. When using variable-length
            // kernels, we will need to pass zero as the input length:
            var svm = new KernelSupportVectorMachine(kernel, inputs: 0);

            // Create the Sequential Minimal Optimization learning algorithm
            var smo = new SequentialMinimalOptimization(svm, inputs, outputs)
            {
                Complexity = 1.5
            };


            // And start learning it!
            double error = smo.Run(); // error will be 0.0


            // At this point, we should have obtained an useful machine. Let's
            // see if it can understand a few examples it hasn't seem before:

            double[][] a = 
            { 
                new double[] { 1, 1, 1 },
                new double[] { 7, 2, 5 },
                new double[] { 2, 5, 1 },
            };

            double[][] b =
            {
                new double[] { 7, 5, 2 },
                new double[] { 4, 2, 5 },
                new double[] { 1, 1, 1 },
            };

            // Following the aforementioned logic, sequence (a) should be
            // classified as -1, and sequence (b) should be classified as +1.

            int resultA = System.Math.Sign(svm.Compute(Matrix.Concatenate(a))); // -1
            int resultB = System.Math.Sign(svm.Compute(Matrix.Concatenate(b))); // +1


            Assert.AreEqual(0, error);
            Assert.AreEqual(-1, resultA);
            Assert.AreEqual(+1, resultB);
            
        }

    }
}
