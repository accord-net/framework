// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Darko Jurić, 2013
// https://code.google.com/p/accord/issues/detail?id=27
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

namespace Accord.MachineLearning.Boosting
{
    using System;
    using System.Collections.Generic;
    using Accord.Compat;
    using Accord.Statistics;

    // TODO: Divide into separate classes

    /// <summary>
    ///   Weighted Weak Classifier.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type of the weak classifier.</typeparam>
    /// 
    [Serializable]
    public class Weighted<TModel> : Weighted<TModel, double[]>
           where TModel : IClassifier<double[], int>
    {
    }

    /// <summary>
    ///   Weighted Weak Classifier.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type of the weak classifier.</typeparam>
    /// <typeparam name="TInput">The type of the input vectors accepted by the classifier.</typeparam>
    /// 
    [Serializable]
    public class Weighted<TModel, TInput>
       where TModel : IClassifier<TInput, int>
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
    public class Boost<TModel> : BoostBase<TModel, Weighted<TModel>, double[]>
        where TModel : IClassifier<double[], int>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="Boost&lt;TModel&gt;"/> class.
        /// </summary>
        /// 
        public Boost()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Boost&lt;TModel&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="weights">The initial boosting weights.</param>
        /// <param name="models">The initial weak classifiers.</param>
        /// 
        public Boost(IList<double> weights, IList<TModel> models)
            : base(weights, models)
        {
        }

        /// <summary>
        ///   Computes the output class label for a given input.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// 
        /// <returns>The most likely class label for the given input.</returns>
        /// 
        [Obsolete("Please use the Decide() method instead.")]
        public int Compute(double[] input)
        {
            return Decide(input) ? +1 : -1;
        }
    }


    /// <summary>
    ///   Boosted classification model.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type of the weak classifier.</typeparam>
    /// <typeparam name="TInput">The type of the input vectors accepted by the classifier.</typeparam>
    ///
    [Serializable]
    public class Boost<TModel, TInput> : BoostBase<TModel, Weighted<TModel, TInput>, TInput>
        where TModel : IClassifier<TInput, int>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="Boost&lt;TModel&gt;"/> class.
        /// </summary>
        /// 
        public Boost()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Boost&lt;TModel&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="weights">The initial boosting weights.</param>
        /// <param name="models">The initial weak classifiers.</param>
        /// 
        public Boost(IList<double> weights, IList<TModel> models)
            : base(weights, models)
        {
        }
    }

    /// <summary>
    ///   Boosted classification model.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type of the weak classifier.</typeparam>
    /// <typeparam name="TWeighted">The type of the weighted classifier.</typeparam>
    /// <typeparam name="TInput">The type of the input vectors accepted by the classifier.</typeparam>
    /// 
    [Serializable]
    public class BoostBase<TModel, TWeighted, TInput> : BinaryClassifierBase<TInput>, IEnumerable<TWeighted>
        where TModel : IClassifier<TInput, int>
        where TWeighted : Weighted<TModel, TInput>, new()
    {

        /// <summary>
        ///   Gets the list of weighted weak models
        ///   contained in this boosted classifier.
        /// </summary>
        /// 
        public List<TWeighted> Models { get; private set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Boost&lt;TModel&gt;"/> class.
        /// </summary>
        /// 
        public BoostBase()
        {
            Models = new List<TWeighted>();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Boost&lt;TModel&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="weights">The initial boosting weights.</param>
        /// <param name="models">The initial weak classifiers.</param>
        /// 
        public BoostBase(IList<double> weights, IList<TModel> models)
        {
            if (weights.Count != models.Count)
                throw new DimensionMismatchException("models", "The number of models and weights must match.");

            for (int i = 0; i < weights.Count; i++)
            {
                if (models[i].NumberOfClasses != 2)
                    throw new ArgumentException("Only binary classifiers are supported at this time.");
                Models.Add(new TWeighted() { Weight = weights[i], Model = models[i] });
            }
        }

        /// <summary>
        /// Computes a class-label decision for a given <paramref name="input" />.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// one of the <see cref="P:Accord.MachineLearning.ITransform.NumberOfOutputs" /> possible classes.</param>
        /// <returns>A class-label that best described <paramref name="input" /> according
        /// to this classifier.</returns>
        public override bool Decide(TInput input)
        {
            double sum = 0.0;
            foreach (var pair in Models)
            {
                if (pair.Model.Decide(input) > 0)
                    sum += pair.Weight;
                else
                    sum -= pair.Weight;
            }

            return Classes.Decide(sum);
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
            Models.Add(new TWeighted() { Weight = weight, Model = model });
        }

        /// <summary>
        ///   Gets or sets the <see cref="Accord.MachineLearning.Boosting
        ///   .Weighted&lt;TModel&gt;"/> at the specified index.
        /// </summary>
        /// 
        public TWeighted this[int index]
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
        public IEnumerator<TWeighted> GetEnumerator()
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
