﻿// Accord.NET Sample Applications
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
using Accord.IO;
using Accord.MachineLearning;
using Accord.Math;
using Accord.Math.Distances;
using Accord.Statistics.Distributions.DensityKernels;
using Accord.Statistics.Kernels;
using System.Data;

namespace Tutorials.Clustering.Hard
{

    class Program
    {
        static void Main(string[] args)
        {
            // In the previous chapters, we have seen how the many models in the Accord.NET Framework 
            // could be used used to solve classification and regression problems. A common aspect in
            // these kinds of problems is that our target output data is always available: be it a set
            // of class labels in the case of classification, or a set of real values in the case of
            // regression. However, there is a different family of problems in which neither of those
            // are available: clustering.

            // In a clustering problem, we would like to extract some useful information about a data
            // set without having exact knowledge about it. A typical application for clustering is in
            // user profiling: let's say we would like to discover groups of users who behave in similar
            // ways when interacting with a system. If we had a database with details on user interaction,
            // we could use a clustering algorithm to "cluster" (group) together people that behave in
            // the same way.

            DataTable table = new ExcelReader("examples.xls").GetWorksheet("Scholkopf");

            double[][] inputs = table.ToJagged().GetColumns(0, 1);

            // Plot the data
            ScatterplotBox.Show("Three groups", inputs).Hold();


            kmeans(inputs);

            binarySplit(inputs);

            meanShift(inputs);
        }

        private static void kmeans(double[][] inputs)
        {
            // Create a 3-Means algorithm
            var kmeans = new KMeans(k: 3)
            {
                Distance = new SquareEuclidean(),
                MaxIterations = 1000
            };

            // Use it to learn a data model
            var model = kmeans.Learn(inputs);

            // Use the model to group new instances
            int[] prediction = model.Decide(inputs);

            // Plot the results
            ScatterplotBox.Show("KMeans's answer", inputs, prediction).Hold();
        }

        private static void binarySplit(double[][] inputs)
        {
            // Create a binary-split algorithm
            var binarySplit = new BinarySplit(k: 3)
            {
                Distance = new SquareEuclidean(),
                MaxIterations = 1000
            };

            // Use it to learn a data model
            var model = binarySplit.Learn(inputs);

            // Use the model to group new instances
            int[] prediction = model.Decide(inputs);

            // Plot the results
            ScatterplotBox.Show("Binary Split's answer", inputs, prediction).Hold();
        }

        private static void meanShift(double[][] inputs)
        {
            // Create a mean-shfit algorithm
            var kmeans = new MeanShift()
            {
                Bandwidth = 0.1,
                Kernel = new EpanechnikovKernel(),
                Distance = new Euclidean(),
                MaxIterations = 1000
            };

            // Use it to learn a data model
            var model = kmeans.Learn(inputs);

            // Use the model to group new instances
            int[] prediction = model.Decide(inputs);

            // Plot the results
            ScatterplotBox.Show("Mean-Shift's answer", inputs, prediction).Hold();
        }
    }
}
