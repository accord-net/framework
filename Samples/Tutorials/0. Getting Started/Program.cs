// Accord.NET Sample Applications
// http://accord-framework.net
//
// Copyright © 2009-2017, César Souza
// All rights reserved. 3-BSD License:
//
//   Redistribution and use in source and binary forms, with or without
//   modification, are permitted provided that the following conditions are met:
//
//      * Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer.
//
//      * Redistributions in binary form must reproduce the above copyright
//        notice, this list of conditions and the following disclaimer in the
//        documentation and/or other materials provided with the distribution.
//
//      * Neither the name of the Accord.NET Framework authors nor the
//        names of its contributors may be used to endorse or promote products
//        derived from this software without specific prior written permission.
// 
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 

using Accord.Controls;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math.Optimization.Losses;
using Accord.Statistics;
using Accord.Statistics.Kernels;
using System;

namespace GettingStarted
{
    class Program
    {
        [MTAThread]
        static void Main(string[] args)
        {
            double[][] inputs =
            {
                /* 1.*/ new double[] { 0, 0 },
                /* 2.*/ new double[] { 1, 0 }, 
                /* 3.*/ new double[] { 0, 1 }, 
                /* 4.*/ new double[] { 1, 1 },
            };

            int[] outputs =
            { 
                /* 1. 0 xor 0 = 0: */ 0,
                /* 2. 1 xor 0 = 1: */ 1,
                /* 3. 0 xor 1 = 1: */ 1,
                /* 4. 1 xor 1 = 0: */ 0,
            };

            // Create the learning algorithm with the chosen kernel
            var smo = new SequentialMinimalOptimization<Gaussian>()
            {
                Complexity = 100 // Create a hard-margin SVM 
            };

            // Use the algorithm to learn the svm
            var svm = smo.Learn(inputs, outputs);

            // Compute the machine's answers for the given inputs
            bool[] prediction = svm.Decide(inputs);

            // Compute the classification error between the expected 
            // values and the values actually predicted by the machine:
            double error = new AccuracyLoss(outputs).Loss(prediction);

            Console.WriteLine("Error: " + error);

            // Show results on screen 
            ScatterplotBox.Show("Training data", inputs, outputs);
            ScatterplotBox.Show("SVM results", inputs, prediction.ToZeroOne());

            Console.ReadKey();
        }
    }
}