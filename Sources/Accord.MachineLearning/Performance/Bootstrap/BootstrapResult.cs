﻿// Accord Machine Learning Library
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
    using Accord.MachineLearning.Performance;
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    ///   Obsolete. Please refer to <see cref="Bootstrap{TModel, TInput, TOutput}"/> instead.
    /// </summary>
    /// 
    [Obsolete("Please use Bootstrap<TModel, TInput, TOutput> instead.")]
    public class BootstrapResult
    {
        /// <summary>
        ///   Gets the <see cref="Bootstrap"/>   
        ///   object used to generate this result.
        /// </summary>
        /// 
        public Bootstrap Settings { get; private set; }

        /// <summary>
        ///   Gets the performance statistics for the training set.
        /// </summary>
        /// 
        public CrossValidationStatistics Training { get; private set; }

        /// <summary>
        ///   Gets the performance statistics for the validation set.
        /// </summary>
        public CrossValidationStatistics Validation { get; private set; }

        /// <summary>
        ///   Gets the 0.632 bootstrap estimate.
        /// </summary>
        /// 
        public double Estimate { get; private set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="BootstrapResult"/> class.
        /// </summary>
        /// 
        /// <param name="owner">The <see cref="Bootstrap"/> that is creating this result.</param>
        /// <param name="models">The models created during the cross-validation runs.</param>
        /// 
        public BootstrapResult(Bootstrap owner, BootstrapValues[] models)
        {
            double[] trainingValues = new double[models.Length];
            double[] trainingVariances = new double[models.Length];
            int[] trainingCount = new int[models.Length];

            double[] validationValues = new double[models.Length];
            double[] validationVariances = new double[models.Length];
            int[] validationCount = new int[models.Length];

            for (int i = 0; i < models.Length; i++)
            {
                trainingValues[i] = models[i].TrainingValue;
                trainingVariances[i] = models[i].TrainingVariance;

                validationValues[i] = models[i].ValidationValue;
                validationVariances[i] = models[i].ValidationVariance;

                owner.GetPartitionSize(i, out trainingCount[i], out validationCount[i]);
            }

            this.Settings = owner;
            this.Training = new CrossValidationStatistics(trainingCount, trainingValues, trainingVariances);
            this.Validation = new CrossValidationStatistics(validationCount, validationValues, validationVariances);

            this.Estimate = 0.632 * Validation.Mean + 0.368 * Training.Mean;
        }

#if !NETSTANDARD1_4
        /// <summary>
        ///   Saves the result to a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to which the result is to be serialized.</param>
        /// 
        public void Save(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(stream, this);
        }

        /// <summary>
        ///   Saves the result to a stream.
        /// </summary>
        /// 
        /// <param name="path">The stream to which the result is to be serialized.</param>
        /// 
        public void Save(string path)
        {
            Save(new FileStream(path, FileMode.Create));
        }

        /// <summary>
        ///   Loads a result from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the result is to be deserialized.</param>
        /// 
        /// <returns>The deserialized result.</returns>
        /// 
        public static BootstrapResult Load(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            return (BootstrapResult)b.Deserialize(stream);
        }

        /// <summary>
        ///   Loads a result from a stream.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the result is to be deserialized.</param>
        /// 
        /// <returns>The deserialized result.</returns>
        /// 
        public static BootstrapResult Load(string path)
        {
            return Load(new FileStream(path, FileMode.Open));
        }
#endif
    }
}
