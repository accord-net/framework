// Accord Statistics Library
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
    using System;
    using System.Runtime.Serialization;
    using Accord.Compat;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///   Base class for parallel learning algorithms.
    /// </summary>
    /// 
    [Serializable]
    public class ParallelLearningBase : IParallel
    {
        [NonSerialized]
        private ParallelOptions parallelOptions;

        /// <summary>
        ///   Gets or sets the parallelization options for this algorithm.
        /// </summary>
        /// 
        public ParallelOptions ParallelOptions
        {
            get { return parallelOptions; }
            set { parallelOptions = value; }
        }

        /// <summary>
        /// Gets or sets a cancellation token that can be used
        /// to cancel the algorithm while it is running.
        /// </summary>
        /// 
        public CancellationToken Token
        {
            get { return ParallelOptions.CancellationToken; }
            set { ParallelOptions.CancellationToken = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParallelLearningBase"/> class.
        /// </summary>
        public ParallelLearningBase()
        {
            parallelOptions = new ParallelOptions();
        }

        /// <summary>
        ///   Called when the object is being deserialized.
        /// </summary>
        /// 
        [OnDeserialized]
        private void OnDeserializedMethod(StreamingContext context)
        {
            parallelOptions = new ParallelOptions();
        }
    }
}
