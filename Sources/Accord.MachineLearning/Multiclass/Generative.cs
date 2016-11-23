// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    ///   Base class for multi-label classifiers based on the 
    ///   generative construction based on generative models.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type for the inner generative model.</typeparam>
    /// <typeparam name="TInput">The input type handled by the classifiers. Default is double.</typeparam>
    /// 
    [Serializable]
    public class Generative<TModel, TInput> : MulticlassLikelihoodClassifierBase<TInput>
        where TModel : IGenerative<TInput>
    {
        private TModel[] models;
        private double[] weights;

        /// <summary>
        ///   Initializes a new instance of the <see cref="Generative{TBinary, TInput}"/> class.
        /// </summary>
        /// 
        /// <param name="classes">The number of classes in the multi-label classification problem.</param>
        /// <param name="initializer">A function to create the inner binary classifiers.</param>
        /// 
        protected Generative(int classes, Func<TModel> initializer)
        {
            this.NumberOfOutputs = classes;
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
        ///   Gets the generative model for a particular class index.
        /// </summary>
        /// 
        /// <param name="classIndex">Index of the class.</param>
        /// 
        /// <returns>
        ///   A <see cref="IGenerative{T}"/> that has been
        ///   trained to recognize samples from the chosen class.
        /// </returns>
        /// 
        public TModel GetModelForClass(int classIndex)
        {
            return models[classIndex];
        }

        /// <summary>
        ///   Gets or sets the generative model for a particular class index.
        /// </summary>
        /// 
        /// <param name="classIndex">Index of the class.</param>
        /// 
        /// <returns>
        ///   A <see cref="IGenerative{T}"/> that has been
        ///   trained to recognize samples from the chosen class.
        /// </returns>
        /// 
        public TModel this[int classIndex]
        {
            get { return models[classIndex]; }
            set { models[classIndex] = value; }
        }

        public double[] Weights
        {
            get { return weights; }
            set { weights = value; }
        }

        /// <summary>
        ///   Gets or sets the generative models that have been trained
        ///   to recognize samples from each class.
        /// </summary>
        /// 
        public TModel[] Models
        {
            get { return models; }
            set { models = value; }
        }



        public override double LogLikelihood(TInput input, int classIndex)
        {
            return models[classIndex].LogLikelihood(input) + Math.Log(weights[classIndex]);
        }


      
    }

}
