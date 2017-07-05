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
    using Accord.MachineLearning.Geometry;
    using NUnit.Framework;
    using System;
    using Accord.Math.Geometry;
    using Accord.Math;
    using Accord.Statistics.Models.Regression.Linear;
    using Accord.MachineLearning;
    using System.Collections.Generic;

    [TestFixture]
    public class RansacTest
    {

        [Test]
        public void zero_inliers_test()
        {
            // Fix the random number generator
            Accord.Math.Random.Generator.Seed = 0;

            double[,] data = // This is the same data used in the RANSAC sample app
            {
                {  1.0,  0.79 }, {  3,    2.18 }, {  5,    5.99 }, {  7.0,  7.65 },
                {  9.0,  9.55 }, { 11,   11.89 }, { 13,   13.73 }, { 15.0, 14.77 }, 
                { 17.0, 18.00 }, {  1.2,  1.45 }, {  1.5,  1.18 }, {  1.8,  1.92 },
                {  2.1,  1.47 }, {  2.4,  2.41 }, {  2.7,  2.35 }, {  3.0,  3.41 },
                {  3.3,  3.78 }, {  3.6,  3.21 }, {  3.9,  4.76 }, {  4.2,  5.03 },
                {  4.5,  4.19 }, {  4.8,  3.81 }, {  5.1,  6.07 }, {  5.4,  5.74 },
                {  5.7,  6.39 }, {  6,    6.11 }, {  6.3,  6.86 }, {  6.6,  6.35 },
                {  6.9,  7.9  }, {  7.2,  8.04 }, {  7.5,  8.48 }, {  7.8,  8.07 },
                {  8.1,  8.22 }, {  8.4,  8.41 }, {  8.7,  9.4  }, {  9,    8.8 },
                {  9.3,  8.44 }, {  9.6,  9.32 }, {  9.9,  9.18 }, { 10.2,  9.86 },
                { 10.5, 10.16 }, { 10.8, 10.28 }, { 11.1, 11.07 }, { 11.4,  11.66 },
                { 11.7, 11.13 }, { 12,   11.55 }, { 12.3, 12.62 }, { 12.6,  12.27 },
                { 12.9, 12.33 }, { 13.2, 12.37 }, { 13.5, 12.75 }, { 13.8,  14.44 },
                { 14.1, 14.71 }, { 14.4, 13.72 }, { 14.7, 14.54 }, { 15,    14.67 },
                { 15.3, 16.04 }, { 15.6, 15.21 }, {  1,    3.9  }, {  2,    11.5 },
                {  3.0, 13.0  }, {  4,    0.9  }, {  5,    5.5  }, {  6,    16.2 },
                {  7.0,  0.8  }, {  8,    9.4  }, {  9,    9.5  }, { 10,    17.5 },
                { 11.0,  6.3  }, { 12,   12.6  }, { 13,    1.5  }, { 14,     1.5 },
                {  2.0,  10   }, {  3,    9    }, { 15,    2    }, { 15.5,   1.2 },
            };


            // First, fit simple linear regression directly for comparison reasons.
            double[] x = data.GetColumn(0); // Extract the independent variable
            double[] y = data.GetColumn(1); // Extract the dependent variable

            // Create a simple linear regression
            var regression = new SimpleLinearRegression();
            Assert.AreEqual(1 , regression.NumberOfInputs);
            Assert.AreEqual(1, regression.NumberOfOutputs);

            // Estimate a line passing through the (x, y) points
            double sumOfSquaredErrors = regression.Regress(x, y);

            // Now, compute the values predicted by the 
            // regression for the original input points
            double[] commonOutput = regression.Compute(x);

            // Now, fit simple linear regression using RANSAC
            int maxTrials = 1000;
            int minSamples = 20;
            double probability = 0.950;
            double errorThreshold = 1000;

            int count = 0;

            // Create a RANSAC algorithm to fit a simple linear regression
            var ransac = new RANSAC<SimpleLinearRegression>(minSamples)
            {
                Probability = probability,
                Threshold = errorThreshold,
                MaxEvaluations = maxTrials,

                // Define a fitting function
                Fitting = delegate(int[] sample)
                {
                    // Retrieve the training data
                    double[] inputs = x.Submatrix(sample);
                    double[] outputs = y.Submatrix(sample);

                    // Build a Simple Linear Regression model
                    var r = new SimpleLinearRegression();
                    r.Regress(inputs, outputs);
                    return r;
                },

                // Define a check for degenerate samples
                Degenerate = delegate(int[] sample)
                {
                    // In this case, we will not be performing such checks.
                    return false;
                },

                // Define a inlier detector function
                Distances = delegate(SimpleLinearRegression r, double threshold)
                {
                    count++;

                    List<int> inliers = new List<int>();

                    // Generate 0 inliers twice, then proceed as normal
                    if (count > 2)
                    {
                        for (int i = 0; i < x.Length; i++)
                        {
                            // Compute error for each point
                            double error = r.Compute(x[i]) - y[i];

                            // If the squared error is below the given threshold,
                            //  the point is considered to be an inlier.
                            if (error * error < threshold)
                                inliers.Add(i);
                        }
                    }

                    return inliers.ToArray();
                }
            };


            // Now that the RANSAC hyperparameters have been specified, we can 
            // compute another regression model using the RANSAC algorithm:

            int[] inlierIndices;
            SimpleLinearRegression robustRegression = ransac.Compute(data.Rows(), out inlierIndices);


            // Compute the output of the model fitted by RANSAC
            double[] ransacOutput = robustRegression.Compute(x);

            Assert.AreEqual(ransac.TrialsNeeded, 0);
            Assert.AreEqual(ransac.TrialsPerformed, 3);

            string a = inlierIndices.ToCSharp();
            string b = ransacOutput.ToCSharp();
            int[] expectedInliers = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75 };
            double[] expectedOutput = new double[] { 4.62124895918799, 5.37525473445784, 6.12926050972769, 6.88326628499754, 7.63727206026739, 8.39127783553724, 9.14528361080709, 9.89928938607694, 10.6532951613468, 4.69664953671498, 4.80975040300545, 4.92285126929593, 5.03595213558641, 5.14905300187689, 5.26215386816736, 5.37525473445784, 5.48835560074832, 5.6014564670388, 5.71455733332927, 5.82765819961975, 5.94075906591023, 6.05385993220071, 6.16696079849118, 6.28006166478166, 6.39316253107214, 6.50626339736262, 6.61936426365309, 6.73246512994357, 6.84556599623405, 6.95866686252453, 7.071767728815, 7.18486859510548, 7.29796946139596, 7.41107032768644, 7.52417119397691, 7.63727206026739, 7.75037292655787, 7.86347379284835, 7.97657465913882, 8.0896755254293, 8.20277639171978, 8.31587725801026, 8.42897812430073, 8.54207899059121, 8.65517985688169, 8.76828072317216, 8.88138158946264, 8.99448245575312, 9.1075833220436, 9.22068418833408, 9.33378505462455, 9.44688592091503, 9.55998678720551, 9.67308765349599, 9.78618851978646, 9.89928938607694, 10.0123902523674, 10.1254911186579, 4.62124895918799, 4.99825184682292, 5.37525473445784, 5.75225762209277, 6.12926050972769, 6.50626339736262, 6.88326628499754, 7.26026917263247, 7.63727206026739, 8.01427494790232, 8.39127783553724, 8.76828072317216, 9.14528361080709, 9.52228649844202, 4.99825184682292, 5.37525473445784, 9.89928938607694, 10.0877908298944 };

            Assert.IsTrue(inlierIndices.IsEqual(expectedInliers));
            Assert.IsTrue(ransacOutput.IsEqual(expectedOutput, 1e-10));
        }

        [Test]
        public void new_api_test()
        {
            #region doc_learn
            // Fix the random number generator
            Accord.Math.Random.Generator.Seed = 0;

            double[,] data = // This is the same data used in the RANSAC sample app
            {
                {  1.0,  0.79 }, {  3,    2.18 }, {  5,    5.99 }, {  7.0,  7.65 },
                {  9.0,  9.55 }, { 11,   11.89 }, { 13,   13.73 }, { 15.0, 14.77 }, 
                { 17.0, 18.00 }, {  1.2,  1.45 }, {  1.5,  1.18 }, {  1.8,  1.92 },
                {  2.1,  1.47 }, {  2.4,  2.41 }, {  2.7,  2.35 }, {  3.0,  3.41 },
                {  3.3,  3.78 }, {  3.6,  3.21 }, {  3.9,  4.76 }, {  4.2,  5.03 },
                {  4.5,  4.19 }, {  4.8,  3.81 }, {  5.1,  6.07 }, {  5.4,  5.74 },
                {  5.7,  6.39 }, {  6,    6.11 }, {  6.3,  6.86 }, {  6.6,  6.35 },
                {  6.9,  7.9  }, {  7.2,  8.04 }, {  7.5,  8.48 }, {  7.8,  8.07 },
                {  8.1,  8.22 }, {  8.4,  8.41 }, {  8.7,  9.4  }, {  9,    8.8 },
                {  9.3,  8.44 }, {  9.6,  9.32 }, {  9.9,  9.18 }, { 10.2,  9.86 },
                { 10.5, 10.16 }, { 10.8, 10.28 }, { 11.1, 11.07 }, { 11.4,  11.66 },
                { 11.7, 11.13 }, { 12,   11.55 }, { 12.3, 12.62 }, { 12.6,  12.27 },
                { 12.9, 12.33 }, { 13.2, 12.37 }, { 13.5, 12.75 }, { 13.8,  14.44 },
                { 14.1, 14.71 }, { 14.4, 13.72 }, { 14.7, 14.54 }, { 15,    14.67 },
                { 15.3, 16.04 }, { 15.6, 15.21 }, {  1,    3.9  }, {  2,    11.5 },
                {  3.0, 13.0  }, {  4,    0.9  }, {  5,    5.5  }, {  6,    16.2 },
                {  7.0,  0.8  }, {  8,    9.4  }, {  9,    9.5  }, { 10,    17.5 },
                { 11.0,  6.3  }, { 12,   12.6  }, { 13,    1.5  }, { 14,     1.5 },
                {  2.0,  10   }, {  3,    9    }, { 15,    2    }, { 15.5,   1.2 },
            };


            // First, fit simple linear regression directly for comparison reasons.
            double[] x = data.GetColumn(0); // Extract the independent variable
            double[] y = data.GetColumn(1); // Extract the dependent variable

            // Use Ordinary Least Squares to learn the regression
            OrdinaryLeastSquares ols = new OrdinaryLeastSquares();

            // Estimate a line passing through the (x, y) points
            SimpleLinearRegression regression = ols.Learn(x, y);

            // Now, compute the values predicted by the 
            // regression for the original input points
            double[] commonOutput = regression.Transform(x);


            // Now, fit simple linear regression using RANSAC
            int maxTrials = 1000;
            int minSamples = 20;
            double probability = 0.950;
            double errorThreshold = 1000;

            // Create a RANSAC algorithm to fit a simple linear regression
            var ransac = new RANSAC<SimpleLinearRegression>(minSamples)
            {
                Probability = probability,
                Threshold = errorThreshold,
                MaxEvaluations = maxTrials,

                // Define a fitting function
                Fitting = (int[] sample) =>
                {
                    // Build a Simple Linear Regression model
                    return new OrdinaryLeastSquares()
                        .Learn(x.Get(sample), y.Get(sample));
                },

                // Define a inlier detector function
                Distances = (SimpleLinearRegression r, double threshold) =>
                {
                    var inliers = new List<int>();
                    for (int i = 0; i < x.Length; i++)
                    {
                        // Compute error for each point
                        double error = r.Transform(x[i]) - y[i];

                        // If the square error is low enough,
                        if (error * error < threshold)
                            inliers.Add(i); //  the point is considered an inlier.
                    }

                    return inliers.ToArray();
                }
            };


            // Now that the RANSAC hyperparameters have been specified, we can 
            // compute another regression model using the RANSAC algorithm:

            int[] inlierIndices;
            SimpleLinearRegression robustRegression = ransac.Compute(data.Rows(), out inlierIndices);

            // Compute the output of the model fitted by RANSAC
            double[] ransacOutput = robustRegression.Transform(x);

            #endregion

            Assert.AreEqual(ransac.TrialsNeeded, 0);
            Assert.AreEqual(ransac.TrialsPerformed, 1);

            string a = inlierIndices.ToCSharp();
            string b = ransacOutput.ToCSharp();
            int[] expectedInliers = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75 };
            double[] expectedOutput = new double[] { 1.96331236445045, 3.42042856976283, 4.87754477507521, 6.33466098038758, 7.79177718569996, 9.24889339101234, 10.7060095963247, 12.1631258016371, 13.6202420069495, 2.10902398498169, 2.32759141577855, 2.5461588465754, 2.76472627737226, 2.98329370816912, 3.20186113896597, 3.42042856976283, 3.63899600055969, 3.85756343135654, 4.0761308621534, 4.29469829295026, 4.51326572374711, 4.73183315454397, 4.95040058534082, 5.16896801613768, 5.38753544693454, 5.6061028777314, 5.82467030852825, 6.04323773932511, 6.26180517012196, 6.48037260091882, 6.69894003171568, 6.91750746251253, 7.13607489330939, 7.35464232410625, 7.5732097549031, 7.79177718569996, 8.01034461649682, 8.22891204729367, 8.44747947809053, 8.66604690888738, 8.88461433968424, 9.1031817704811, 9.32174920127795, 9.54031663207481, 9.75888406287167, 9.97745149366852, 10.1960189244654, 10.4145863552622, 10.6331537860591, 10.8517212168559, 11.0702886476528, 11.2888560784497, 11.5074235092465, 11.7259909400434, 11.9445583708402, 12.1631258016371, 12.3816932324339, 12.6002606632308, 1.96331236445045, 2.69187046710664, 3.42042856976283, 4.14898667241902, 4.87754477507521, 5.6061028777314, 6.33466098038758, 7.06321908304377, 7.79177718569996, 8.52033528835615, 9.24889339101234, 9.97745149366852, 10.7060095963247, 11.4345676989809, 2.69187046710664, 3.42042856976283, 12.1631258016371, 12.5274048529652 };

            Assert.IsTrue(inlierIndices.IsEqual(expectedInliers));
            Assert.IsTrue(ransacOutput.IsEqual(expectedOutput, 1e-10));
        }

    }
}
