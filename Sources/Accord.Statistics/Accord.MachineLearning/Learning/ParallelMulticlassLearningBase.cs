// Accord Statistics Library
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
    using Accord.MachineLearning;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///   Base class for multi-class learning algorithms that employ parallel algorithms.
    /// </summary>
    /// 
    public abstract class ParallelMulticlassLearningBase<TModel, TInput> :  
        MulticlassLearningBase<TModel, TInput>, IParallel
        where TModel : IMulticlassClassifier<TInput>
    {
        /// <summary>
        ///   Gets or sets the parallelization options for this learning algorithm.
        /// </summary>
        /// 
        public ParallelOptions ParallelOptions { get; set; }

        /// <summary>
        /// Gets or sets a cancellation token that can be used
        /// to cancel the algorithm while it is running.
        /// </summary>
        /// 
        public override CancellationToken Token
        {
            get { return ParallelOptions.CancellationToken; }
            set { ParallelOptions.CancellationToken = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParallelMulticlassLearningBase{TModel, TInput}"/> class.
        /// </summary>
        protected ParallelMulticlassLearningBase()
        {
            ParallelOptions = new ParallelOptions();
        }
    }
}
