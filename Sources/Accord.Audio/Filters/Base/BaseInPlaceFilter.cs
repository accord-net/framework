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

namespace Accord.Audio.Filters
{
    using System;
    using System.Collections.Generic;
    using Accord.Compat;

    /// <summary>
    ///   Base in-place signal processing filter
    /// </summary>
    /// 
    [Serializable]
    public abstract class BaseInPlaceFilter : IInPlaceFilter, IFilter
    {

        private readonly Dictionary<SampleFormat, SampleFormat> formatTranslations =
            new Dictionary<SampleFormat, SampleFormat>();

        /// <summary>
        ///   Format translations dictionary.
        /// </summary>
        /// 
        /// <value>The format translations.</value>
        /// 
        /// <remarks>
        ///   The dictionary defines which sample formats are supported for
        ///   source signals and which sample format will be used for resulting signal.
        /// </remarks>
        /// 
        public Dictionary<SampleFormat, SampleFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }


        /// <summary>
        ///   Applies the filter to a signal.
        /// </summary>
        /// 
        public Signal Apply(Signal signal)
        {
            // check pixel format of the source signal
            CheckSourceFormat(signal.SampleFormat);

            // get number of channels and samples
            int channels = signal.Channels;
            int samples = signal.Length;

            // retrieve other information
            int rate = signal.SampleRate;

            // destination sample format
            SampleFormat dstSampleFormat = FormatTranslations[signal.SampleFormat];

            // create new signal of required format
            Signal dstSignal = NewSignal(channels, samples, rate, dstSampleFormat);

            // process the filter
            ProcessFilter(signal, dstSignal);

            // return the processed signal
            return dstSignal;
        }

        /// <summary>
        ///   Apply filter to an audio signal.
        /// </summary>
        /// 
        /// <param name="signal">Source signal to apply filter to.</param>
        /// 
        /// <remarks>
        ///   The method alters the original signal to store
        ///   the result of this signal processing filter.
        /// </remarks>
        /// 
        public void ApplyInPlace(Signal signal)
        {
            ProcessFilter(signal, signal);
        }

        /// <summary>
        ///   Creates a new signal from the given signal parameters. This
        ///   method can be overridden on child classes to modify how
        ///   output signals are created.
        /// </summary>
        /// 
        protected virtual Signal NewSignal(int channels, int samples, int rate, SampleFormat dstSampleFormat)
        {
            return new Signal(channels, samples, rate, dstSampleFormat);
        }

        /// <summary>
        ///   Applies the filter to a windowed signal.
        /// </summary>
        /// 
        public Signal[] Apply(params Signal[] signal)
        {
            Signal[] s = new Signal[signal.Length];
            for (int i = 0; i < signal.Length; i++)
                s[i] = Apply(signal[i]);
            return s;
        }

        /// <summary>
        ///   Apply filter to a windowed audio signal.
        /// </summary>
        /// 
        /// <param name="signal">Source signal to apply filter to.</param>
        /// 
        /// <remarks>
        ///   The method alters the original signal to store
        ///   the result of this signal processing filter.
        /// </remarks>
        /// 
        public void ApplyInPlace(params Signal[] signal)
        {
            for (int i = 0; i < signal.Length; i++)
                ApplyInPlace(signal[i]);
        }

        /// <summary>
        ///   Processes the filter.
        /// </summary>
        /// 
        protected abstract void ProcessFilter(Signal sourceData, Signal destinationData);

        // Check pixel format of the source signal
        private void CheckSourceFormat(SampleFormat sampleFormat)
        {
            if (!FormatTranslations.ContainsKey(sampleFormat))
                throw new UnsupportedSampleFormatException("Source sample format is not supported by the filter.");
        }
    }
}
