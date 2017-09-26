// Accord Imaging Library
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

namespace Accord.Imaging
{
    using Accord.Statistics.Distributions.Univariate;
    using System;

    /// <summary>
    ///   Codebook learning statistics for <see cref="BagOfVisualWords"/> models.
    /// </summary>
    /// 
    [Serializable]
    public class BagOfVisualWordsStatistics
    {
        /// <summary>
        ///   Gets or sets the number of images in the training set.
        /// </summary>
        /// 
        /// <value>The number of images.</value>
        /// 
        public int TotalNumberOfImages { get; set; }

        /// <summary>
        ///   Gets or sets the total number of descriptors seen in the training set.
        /// </summary>
        /// 
        /// <value>The total number of descriptors.</value>
        /// 
        public int TotalNumberOfDescriptors { get; set; }

        /// <summary>
        ///   Gets or sets the count distribution of the descriptors seen in the training set.
        /// </summary>
        /// 
        public NormalDistribution TotalNumberOfDescriptorsPerImage { get; set; }

        /// <summary>
        ///   Gets or sets the minimum and maximum number of descriptors per image seen in the training set.
        /// </summary>
        /// 
        public IntRange TotalNumberOfDescriptorsPerImageRange { get; set; }


        /// <summary>
        ///   Gets or sets the number of images actually used in
        ///   the learning of the <see cref="BagOfVisualWords"/>.
        /// </summary>
        /// 
        /// <value>The number of images.</value>
        /// 
        public int NumberOfImagesTaken { get; set; }

        /// <summary>
        ///   Gets or sets the number of descriptors actually used 
        ///   in the learning of the <see cref="BagOfVisualWords"/>.
        /// </summary>
        /// 
        /// <value>The total number of descriptors.</value>
        /// 
        public int NumberOfDescriptorsTaken { get; set; }

        /// <summary>
        ///   Gets or sets the count distribution of the descriptors actually
        ///   used in the learning of the <see cref="BagOfVisualWords"/>.
        /// </summary>
        /// 
        public NormalDistribution NumberOfDescriptorsTakenPerImage { get; set; }

        /// <summary>
        ///   Gets or sets the minimum and maximum number of descriptors per image
        ///   actually used in the learning of the <see cref="BagOfVisualWords"/>.
        /// </summary>
        /// 
        public IntRange NumberOfDescriptorsTakenPerImageRange { get; set; }
    }
}