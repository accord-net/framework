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

namespace Accord.MachineLearning.Boosting.Learners
{
    using System;
    using Accord.Math.Comparers;
    using Accord.Math;

    /// <summary>
    ///   Simple classifier that based on decision margins that
    ///   are perpendicular to one of the space dimensions.
    /// </summary>
    /// 
    public class Weak<TModel> : IWeakClassifier
    {
        public TModel Model { get; set; }

        public Func<TModel, double[], int> Function { get; set; }

        public Weak(TModel model, Func<TModel, double[], int> function)
        {
            this.Model = model;
            this.Function = function;
        }

        public int Compute(double[] inputs)
        {
            return Function(Model, inputs);
        }
    }
}
