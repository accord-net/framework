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
    using Accord.MachineLearning.VectorMachines;
    using Accord.Statistics.Filters;
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Bag of words.
    /// </summary>
    /// 
    /// <remarks>
    ///   The bag-of-words (BoW) model can be used to extract finite
    ///   length features from otherwise varying length representations.
    /// </remarks>
    /// 
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\BagOfWordsTest.cs" region="doc_learn"/>
    ///   
    /// <para>
    ///   The following example shows how to use Bag-of-Words to convert other kinds of sequences
    ///   into fixed-length representations. In particular, we apply Bag-of-Words to convert data
    ///   from the PENDIGITS handwritten digit recognition dataset and afterwards convert their
    ///   representations using a <see cref="SupportVectorMachine"/>.</para>
    ///   
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\BagOfWordsTest.cs" region="doc_learn_pendigits"/>
    /// </example>
    /// 
    [Serializable]
    public class BagOfWords<TInput> : BaseBagOfWords<BagOfWords<TInput>, TInput, Codification<TInput>.Options>
    {

        /// <summary>
        ///   Constructs a new <see cref="BagOfWords"/>.
        /// </summary>
        /// 
        public BagOfWords()
        {
            Clustering = new Codification<TInput>.Options()
            {
                VariableType = CodificationVariable.Ordinal,
            };
        }
    }
}
