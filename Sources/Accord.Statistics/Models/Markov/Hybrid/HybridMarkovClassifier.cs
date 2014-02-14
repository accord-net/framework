// Accord Statistics Library
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

namespace Accord.Statistics.Models.Markov.Hybrid
{
    using System;
    using Accord.Math;
    using System.Collections.Generic;

   
    /// <summary>
    ///   Hybrid Markov classifier for arbitrary state-observation functions.
    /// </summary>
    /// 
    [Serializable]
    public class HybridMarkovClassifier 
    {

        /// <summary>
        ///   Gets the Markov models for each sequence class.
        /// </summary>
        /// 
        public IHybridMarkovModel[] Models { get; private set; }

        /// <summary>
        ///   Gets the number of dimensions of the 
        ///   observations handled by this classifier.
        /// </summary>
        /// 
        public int Dimension
        {
            get { return Models[0].Dimension; }
        }


        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        /// 
        /// <param name="models">
        ///   The models specializing in each of the classes of 
        ///   the classification problem.</param>
        /// 
        public HybridMarkovClassifier(IEnumerable<IHybridMarkovModel> models)
        {
            this.Models = new List<IHybridMarkovModel>(models).ToArray();
        }

        /// <summary>
        ///   Computes the most likely class for a given sequence.
        /// </summary>
        /// 
        /// <param name="sequence">The sequence of observations.</param>
        /// 
        /// <returns>Return the label of the given sequence, or -1 if it has
        /// been rejected by the <see cref="BaseHiddenMarkovClassifier{T}.Threshold">
        /// threshold model</see>.</returns>
        /// 
        public int Compute(double[][] sequence)
        {
            double[] responses;
            return Compute(sequence, out responses);
        }

        /// <summary>
        ///   Computes the most likely class for a given sequence.
        /// </summary>
        /// 
        /// <param name="sequence">The sequence of observations.</param>
        /// <param name="response">The probability of the assigned class.</param>
        /// 
        /// <returns>Return the label of the given sequence, or -1 if it has
        /// been rejected by the <see cref="BaseHiddenMarkovClassifier{T}.Threshold">
        /// threshold model</see>.</returns>
        /// 
        public int Compute(double[][] sequence, out double response)
        {
            double[] responses;
            int result =  Compute(sequence, out responses);
            response = responses[result];
            return result;
        }

        /// <summary>
        ///   Computes the most likely class for a given sequence.
        /// </summary>
        /// 
        /// <param name="sequence">The sequence of observations.</param>
        /// <param name="responsibilities">The class responsibilities (or
        /// the probability of the sequence to belong to each class). When
        /// using threshold models, the sum of the probabilities will not
        /// equal one, and the amount left was the threshold probability.
        /// If a threshold model is not being used, the array should sum to
        /// one.</param>
        /// 
        /// <returns>Return the label of the given sequence, or -1 if it has
        /// been rejected by the <see cref="BaseHiddenMarkovClassifier{T}.Threshold">
        /// threshold model</see>.</returns>
        /// 
        public int Compute(double[][] sequence, out double[] responsibilities)
        {
            responsibilities = new double[Models.Length];
            for (int i = 0; i < responsibilities.Length; i++)
                responsibilities[i] = Models[i].Evaluate(sequence);

            int imax; responsibilities.Max(out imax);
            return imax;
        }

    }
}
