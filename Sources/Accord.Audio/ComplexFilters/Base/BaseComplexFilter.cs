// Accord Audio Library
// The Accord.NET Framework
// http://accord-framework.net
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

namespace Accord.Audio.ComplexFilters
{

    /// <summary>
    ///   Base complex signal processing filter.
    /// </summary>
    /// 
    public abstract class BaseComplexFilter : IComplexFilter
    {
        
        /// <summary>
        ///   Applies the filter to a signal.
        /// </summary>
        /// 
        public ComplexSignal Apply(ComplexSignal complexSignal)
        {
            // get number of channels and samples
            int channels = complexSignal.Channels;
            int samples = complexSignal.Length;

            // retrieve other information
            int rate = complexSignal.SampleRate;

            // create new signal of required format
            ComplexSignal dstSignal = new ComplexSignal(channels, samples, rate);

            // process the filter
            ProcessFilter(complexSignal, dstSignal);

            // return the processed signal
            return dstSignal;
        }

        /// <summary>
        ///   Applies the filter to a windowed signal.
        /// </summary>
        /// 
        public ComplexSignal[] Apply(params ComplexSignal[] complexSignal)
        {
            ComplexSignal[] s = new ComplexSignal[complexSignal.Length];
            for (int i = 0; i < complexSignal.Length; i++)
                s[i] = Apply(complexSignal[i]);
            return s;
        }

        /// <summary>
        ///   Processes the filter.
        /// </summary>
        /// 
        protected abstract void ProcessFilter(ComplexSignal sourceData, ComplexSignal destinationData);


    }
}
