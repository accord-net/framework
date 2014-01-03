// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Darko Jurić, 2013
// https://code.google.com/p/accord/issues/detail?id=27
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

namespace Accord.MachineLearning.Boosting
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///   Weighted Weak Classifier.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type of the weak classifier.</typeparam>
    /// 
    [Serializable]
    public class Weighted<TModel>
           where TModel : IWeakClassifier
    {
        /// <summary>
        ///   Gets or sets the weight associated
        ///   with the weak <see cref="Model"/>.
        /// </summary>
        /// 
        public double Weight { get; set; }

        /// <summary>
        ///   Gets or sets the <see cref="IWeakClassifier">weak
        ///   classifier</see> associated with the <see cref="Weight"/>.
        /// </summary>
        /// 
        public TModel Model { get; set; }
    }

    /// <summary>
    ///   Boosted classification model.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type of the weak classifier.</typeparam>
    /// 
    [Serializable]
    public class Boost<TModel> : IEnumerable<Weighted<TModel>>
        where TModel : IWeakClassifier
    {

        /// <summary>
        ///   Gets the list of weighted weak models
        ///   contained in this boosted classifier.
        /// </summary>
        /// 
        public List<Weighted<TModel>> Models { get; private set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Boost&lt;TModel&gt;"/> class.
        /// </summary>
        /// 
        public Boost()
        {
            Models = new List<Weighted<TModel>>();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Boost&lt;TModel&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="weights">The initial boosting weights.</param>
        /// <param name="models">The initial weak classifiers.</param>
        /// 
        public Boost(IList<double> weights, IList<TModel> models)
        {
            if (weights.Count != models.Count)
                throw new DimensionMismatchException("models", "The number of models and weights must match.");

            for (int i = 0; i < weights.Count; i++)
                Models.Add(new Weighted<TModel>() { Weight = weights[i], Model = models[i] });
        }

        /// <summary>
        ///   Computes the output class label for a given input.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// 
        /// <returns>The most likely class label for the given input.</returns>
        /// 
        public int Compute(double[] input)
        {
            double sum = 0.0;
            foreach (var pair in Models)
                sum += pair.Weight * pair.Model.Compute(input);

            return System.Math.Sign(sum);
        }

        /// <summary>
        ///   Adds a new weak classifier and its corresponding
        ///   weight to the end of this boosted classifier.
        /// </summary>
        /// 
        /// <param name="weight">The weight of the weak classifier.</param>
        /// <param name="model">The weak classifier</param>
        /// 
        public void Add(double weight, TModel model)
        {
            Models.Add(new Weighted<TModel>() { Weight = weight, Model = model });
        }

        /// <summary>
        ///   Gets or sets the <see cref="Accord.MachineLearning.Boosting
        ///   .Weighted&lt;TModel&gt;"/> at the specified index.
        /// </summary>
        /// 
        public Weighted<TModel> this[int index]
        {
            get { return Models[index]; }
            set { Models[index] = value; }
        }


        /// <summary>
        ///   Returns an enumerator that iterates through this collection.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// 
        public IEnumerator<Weighted<TModel>> GetEnumerator()
        {
            return Models.GetEnumerator();
        }

        /// <summary>
        ///   Returns an enumerator that iterates through this collection.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// 
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Models.GetEnumerator();
        }
    }
}
