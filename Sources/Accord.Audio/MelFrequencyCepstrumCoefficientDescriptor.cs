// Accord Audio Library
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

namespace Accord.Audio
{
    using Accord.Math;
    using System;

    /// <summary>
    ///   Represents a feature vector extracted using <see cref="MelFrequencyCepstrumCoefficient"/>.
    /// </summary>
    /// 
    /// <seealso cref="MelFrequencyCepstrumCoefficient"/>
    /// 
    public struct MelFrequencyCepstrumCoefficientDescriptor : IFeatureDescriptor<double[]>
    {
        private int frame;
        private double[] descriptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MelFrequencyCepstrumCoefficientDescriptor"/> struct.
        /// </summary>
        /// 
        /// <param name="frame">The frame index from which this feature was extracted.</param>
        /// <param name="descriptor">The MFCC descriptor values.</param>
        /// 
        public MelFrequencyCepstrumCoefficientDescriptor(int frame, double[] descriptor)
        {
            this.frame = frame;
            this.descriptor = descriptor;
        }

        /// <summary>
        ///   Gets the frame index from which this feature was extracted.
        /// </summary>
        /// 
        public int Frame
        {
            get { return frame; }
        }

        /// <summary>
        ///   Gets or sets the descriptor vector associated with this point.
        /// </summary>
        /// 
        public double[] Descriptor
        {
            get { return descriptor; }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return String.Format("{0}: {1}", frame, descriptor.ToCSharp());
        }
    }
}
