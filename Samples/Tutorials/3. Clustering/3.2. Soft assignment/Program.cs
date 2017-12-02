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
using Accord.Statistics.Distributions.Fitting;
using Accord.Statistics.Kernels;
using System.Data;

namespace Tutorials.Clustering.Soft
{
    class Program
    {
        static void Main(string[] args)
        {
            // In the previous section, we have seen models that could group training instances together
            // based on their similarity. However, in some problems, we might not want to commit to a hard
            // decision about where each training instance should be placed, but rather we would be more
            // interested on knowing the possibilities and the probability of each training instance falling
            // into each group. In those cases, what we need is a way to perform soft-clustering of the data.
            // 

            // In soft-clustering, instead of outputing a single integer label indicating where each training
            // instance should be put, the model is able to output the probabilities of an instance belonging
            // to the several possible groupings.

            DataTable table = new ExcelReader("examples.xls").GetWorksheet("Scholkopf");

            double[][] inputs = table.ToJagged().GetColumns(0, 1);

            // Plot the data
            ScatterplotBox.Show("Three groups", inputs).Hold();

            gmm(inputs);

            gmmDiagonal(inputs);

            gmmSharedCovariances(inputs);

            gmmRegularization(inputs);

            gmmDegenerate(inputs);
        }

        private static void gmm(double[][] inputs)
        {
            var gmm = new GaussianMixtureModel(components: 3)
            {
                Tolerance = 1e-6
            };

            var model = gmm.Learn(inputs);

            int[] prediction = model.Decide(inputs);

            double[][] prob = model.Probabilities(inputs);

            // Compute the clustering distortion
            double distortion = model.Distortion(inputs, prediction);

            // Plot the results
            ScatterplotBox.Show("Free GMM's answer", inputs, prediction)
                .Hold();
        }

        private static void gmmDiagonal(double[][] inputs)
        {
            var gmm = new GaussianMixtureModel(components: 3)
            {
                Tolerance = 1e-6,
                Options = new NormalOptions()
                {
                    Diagonal = true
                }
            };

            var model = gmm.Learn(inputs);

            int[] prediction = model.Decide(inputs);

            // Compute the clustering distortion
            double distortion = model.Distortion(inputs, prediction);

            // Plot the results
            ScatterplotBox.Show("Diagonal GMM's answer", inputs, prediction)
                .Hold();
        }

        private static void gmmSharedCovariances(double[][] inputs)
        {
            var gmm = new GaussianMixtureModel(components: 3)
            {
                Tolerance = 1e-6,
                Options = new NormalOptions()
                {
                    Shared = true
                }
            };

            var model = gmm.Learn(inputs);

            int[] prediction = model.Decide(inputs);

            // Compute the clustering distortion
            double distortion = model.Distortion(inputs, prediction);

            // Plot the results
            ScatterplotBox.Show("Shared GMM's answer", inputs, prediction)
                .Hold();
        }

        private static void gmmRegularization(double[][] inputs)
        {
            var gmm = new GaussianMixtureModel(components: 10)
            {
                Tolerance = 1e-6,
                Options = new NormalOptions()
                {
                    Regularization = 1e-6
                }
            };

            var model = gmm.Learn(inputs);

            int[] prediction = model.Decide(inputs);

            // Compute the clustering distortion
            double distortion = model.Distortion(inputs, prediction);

            // Plot the results
            ScatterplotBox.Show("Regularized GMM's answer", inputs, prediction)
                .Hold();
        }

        private static void gmmDegenerate(double[][] inputs)
        {
            var gmm = new GaussianMixtureModel(components: 10)
            {
                Tolerance = 1e-4,
                Options = new NormalOptions()
                {
                    Robust = true
                }
            };

            var model = gmm.Learn(inputs);

            int[] prediction = model.Decide(inputs);

            // Compute the clustering distortion
            double distortion = model.Distortion(inputs, prediction);

            // Plot the results
            ScatterplotBox.Show("Robust GMM's answer", inputs, prediction)
                .Hold();
        }

    }
}
