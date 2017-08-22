// Accord Machine Learning Library
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

namespace Accord.MachineLearning
{
    using Accord.Math;
    using System;
    using System.Collections.Generic;
    using Accord.MachineLearning.VectorMachines;
    using System.Linq;
    using System.Text;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Base class for multi-class classifiers based on the 
    ///   "one-vs-rest" construction based on binary classifiers.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type for the inner binary classifiers.</typeparam>
    /// <typeparam name="TInput">The input type handled by the classifiers. Default is double.</typeparam>
    /// 
    /// <seealso cref="MultilabelSupportVectorMachine{TKernel}"/>
    /// 
    [Serializable]
    public class OneVsRest<TModel, TInput> :
        MultilabelLikelihoodClassifierBase<TInput>,
        IEnumerable<KeyValuePair<int, TModel>>
        where TModel : IBinaryScoreClassifier<TInput>
    {
        private TModel[] models;


        /// <summary>
        ///   Initializes a new instance of the <see cref="OneVsRest{TBinary, TInput}"/> class.
        /// </summary>
        /// 
        /// <param name="classes">The number of classes in the multi-label classification problem.</param>
        /// <param name="initializer">A function to create the inner binary classifiers.</param>
        /// 
        protected OneVsRest(int classes, Func<TModel> initializer)
        {
            this.NumberOfOutputs = classes;
            this.NumberOfClasses = classes;
            this.Models = new TModel[classes];
            for (int i = 0; i < Models.Length; i++)
            {
                var model = initializer();
                models[i] = model;
                if (model != null)
                    NumberOfInputs = model.NumberOfInputs;
            }
        }



        /// <summary>
        ///   Gets the binary classifier for particular class index.
        /// </summary>
        /// 
        /// <param name="classIndex">Index of the class.</param>
        /// 
        /// <returns>
        ///   A <see cref="IBinaryClassifier{T}"/> that has been trained
        ///   to distinguish between the chosen class and all other classes.
        /// </returns>
        /// 
        public TModel GetClassifierForClass(int classIndex)
        {
            return models[classIndex];
        }

        /// <summary>
        ///   Gets or sets the inner binary classifiers used to distinguish
        ///   between each class and all other classes.
        /// </summary>
        /// 
        /// <param name="classIndex">The classifier index.</param>
        /// 
        /// <returns>
        ///   A <see cref="IBinaryClassifier{T}"/> that has been trained
        ///   to distinguish between the chosen class and all other classes.
        /// </returns>
        /// 
        public TModel this[int classIndex]
        {
            get { return models[classIndex]; }
            set { models[classIndex] = value; }
        }


        /// <summary>
        ///   Gets or sets the binary classifiers that have been trained
        ///   to distinguish between each class and all other classes.
        /// </summary>
        /// 
        public TModel[] Models
        {
            get { return models; }
            set { models = value; }
        }

        /// <summary>
        ///   Gets the total number of binary models in this one-vs-rest 
        ///   multi-label configuration. Should be equal to the 
        ///   <see cref="ITransform.NumberOfOutputs"/> (number of classes).
        /// </summary>
        /// 
        public int Count
        {
            get { return models.Length; }
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and a given
        /// <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// <param name="decision">The class label associated with the input
        /// vector, as predicted by the classifier.</param>
        /// <returns></returns>
        public override double Score(TInput input, int classIndex, out bool decision)
        {
            return models[classIndex].Score(input, out decision);
        }

        /// <summary>
        /// Computes whether a class label applies to an <paramref name="input" /> vector.
        /// </summary>
        /// <param name="input">The input vectors that should be classified as
        /// any of the <see cref="ITransform.NumberOfOutputs" /> possible classes.</param>
        /// <param name="classIndex">The class label index to be tested.</param>
        /// <returns>
        /// A boolean value indicating whether the given <paramref name="classIndex">
        /// class label</paramref> applies to the <paramref name="input" /> vector.
        /// </returns>
        public override bool Decide(TInput input, int classIndex)
        {
            return ((IClassifier<TInput, bool>)models[classIndex]).Decide(input);
        }

        /// <summary>
        /// Computes a log-likelihood measuring the association between
        /// the given <paramref name="input" /> vector and a given
        /// <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// <param name="decision">The class label associated with the input
        /// vector, as predicted by the classifier.</param>
        public override double LogLikelihood(TInput input, int classIndex, out bool decision)
        {
            var model = models[classIndex] as IBinaryLikelihoodClassifier<TInput>;
            if (model == null)
                throw new NotSupportedException();
            return model.LogLikelihood(input, out decision);
        }



        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<int, TModel>> GetEnumerator()
        {
            for (int i = 0; i < models.Length; i++)
                yield return new KeyValuePair<int, TModel>(i, models[i]);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    ///   Base class for multi-class classifiers based on the 
    ///   "one-vs-rest" construction based on binary classifiers.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type for the inner binary classifiers.</typeparam>
    /// 
    /// <seealso cref="MultilabelSupportVectorMachine{TKernel}"/>
    /// 
    [Serializable]
    public class OneVsRest<TModel> : OneVsRest<TModel, double[]>
       where TModel : IBinaryScoreClassifier<double[]>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="OneVsRest{TBinary}"/> class.
        /// </summary>
        /// 
        /// <param name="classes">The number of classes in the multi-label classification problem.</param>
        /// <param name="initializer">A function to create the inner binary classifiers.</param>
        /// 
        protected OneVsRest(int classes, Func<TModel> initializer)
            : base(classes, initializer)
        {
        }

    }
}
