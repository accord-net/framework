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

using Accord.DataSets;
using Accord.Math;
using Accord.Statistics.Analysis;
using Accord.Statistics.Filters;
using Accord.Statistics.Models.Fields;
using Accord.Statistics.Models.Fields.Functions;
using Accord.Statistics.Models.Fields.Learning;
using Accord.Statistics.Models.Markov.Learning;

namespace Tutorials.Classification.Structured
{
    class Program
    {
        static void Main(string[] args)
        {
            // In the previous sections we have seen how to perform different kinds of classification 
            // tasks: when there are only two classes (binary), when there are more than two classes 
            // but each instance can only be assigned to one class (multi-class), and when there are 
            // more than two classes but and instance can be simultaneously in many classes (multi-label).

            // Yet, all the previous classifiers we saw expect to receive one instance and simply produce 
            // one class assignment for it (be it to one or multiple classes at the same time). However,
            // some classification problems might require us to tell not only which classes an instance
            // might belong to, but also tell how those classes relate to each other. 

            // For example, consider the problem of classifying audio recordings of spoken sentences 
            // according to which words them contain. In this case, since spoken sentences can contain
            // multiple words, we can define one class label after each possible word (restraining ourselves
            // only to words that can be found in an English dictionary), and then cast this problem as
            // a multi-label decision problem which could be solved by the classifiers we saw in the last
            // section.

            // However, consider now that we might interested not only in knowing which words a spoken
            // sentence contains, but also in _which order_ those words appear. In this case, we would
            // need our classifier to output more than a simple class assignment value or a fixed-length 
            // vector indicating which words are in the sentence. We would need the classifier to output
            // an _structure_ that shows how each of those classes related with each other: this structure
            // can be simply a sequence (a chain, or list of class assignments where the class labels can
            // repeat), a tree, a graph, or even other more complex data structures.

            // One of the simplest examples of classifiers that can be used to perform structured classification
            // are Markov models. Given a sequence of observations (like words), these models can learn to classify
            // each word in a sentence by leveraging information about the _order_ in which those words typically 
            // appear. They are then able to produce, for a single training instance (the sequence of words), a 
            // list of class labels (a sequence of assignments) indicating which word is which.

            // We can start by loading the Chunking dataset for Part of Speech tagging:
            var chunking = new Chunking(); // http://www.cnts.ua.ac.be/conll2000/chunking/

            // Learn a mapping between each word to an integer class label:
            var wordMap = new Codification().Learn(chunking.Words);

            // Learn a mapping between each tag to an integer class labels:
            var tagMap = new Codification().Learn(chunking.Tags);

            // Convert the training and testing sets into integer labels:
            int[][] trainX = wordMap.Transform(chunking.Training.Item1);
            int[][] testX = wordMap.Transform(chunking.Testing.Item1);

            // Convert the training and testing tags into integer labels:
            int[][] trainY = tagMap.Transform(chunking.Training.Item2);
            int[][] testY = tagMap.Transform(chunking.Testing.Item2);

            // Learn using a Markov model
            hmm(trainX, trainY, testX, testY);

            // Learn using a Conditional Random Field
            // crf(trainX, trainY, testX, testY);
        }

        private static void hmm(int[][] trainX, int[][] trainY, int[][] testX, int[][] testY)
        {
            // Learn one Markov model using the training data
            var teacher = new MaximumLikelihoodLearning()
            {
                UseLaplaceRule = true,
                UseWeights = true
            };

            // Use the teacher to learn a Markov model
            var markov = teacher.Learn(trainX, trainY);

            // Use the model to predict instances:
            int[][] predY = markov.Decide(testX);

            // Check the accuracy of the model:
            var cm = new ConfusionMatrix(predicted: predY.Concatenate(), expected: testY.Concatenate());
        }

        private static void crf(int[][] trainX, int[][] trainY, int[][] testX, int[][] testY)
        {
            int numberOfClasses = 44; // chunking.Tags.Length;
            int numberOfSymbols = 21589; // chunking.Words.Length;

            // Learn one Markov model using the training data
            var teacher = new QuasiNewtonLearning<int>()
            {
                Function = new MarkovDiscreteFunction(states: numberOfClasses, symbols: numberOfSymbols)
            };

            // Use the teacher to learn a Conditional Random Field model
            ConditionalRandomField<int> crf = teacher.Learn(trainX, trainY);

            // Use the crf to predict instances:
            int[][] predY = crf.Decide(testX);

            // Check the accuracy of the model:
            var cm = new ConfusionMatrix(predicted: predY.Concatenate(), expected: testY.Concatenate());

            double acc = cm.Accuracy;
        }
    }
}
