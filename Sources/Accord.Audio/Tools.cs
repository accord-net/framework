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
    using System.Runtime.InteropServices;
    using System.Collections.Generic;
    using Accord.Compat;
    using System.Numerics;

    /// <summary>
    ///   Tool functions for audio processing.
    /// </summary>
    /// 
    public static class Tools
    {
        /// <summary>
        ///   Interleaves the channels into a single array.
        /// </summary>
        /// 
        public static float[] Interleave(this float[][] channels)
        {
            if (channels == null) throw new ArgumentNullException("channels");
            if (channels.Length == 0) return new float[0];

            int c = channels.Length;
            int n = channels[0].Length;

            float[] data = new float[c * n];

            for (int i = 0, k = 0; i < n; i++)
                for (int j = 0; j < c; j++)
                    data[k++] = channels[j][i];

            return data;
        }

        /// <summary>
        ///   Interleaves the channels into a single array.
        /// </summary>
        /// 
        public static float[] Interleave(this float[,] channels)
        {
            float[] result = new float[channels.Length];
            Buffer.BlockCopy(channels, 0, result, 0, result.Length * sizeof(float));
            return result;
        }

        /// <summary>
        ///   Obsolete. Please use <see cref="Accord.Math.Transforms.FourierTransform2.GetMagnitudeSpectrum(Complex[])"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use Accord.Math.Transforms.FourierTransform2.GetMagnitudeSpectrum(fft) instead.")]
        public static double[] GetMagnitudeSpectrum(Complex[] fft)
        {
            return Accord.Math.Transforms.FourierTransform2.GetMagnitudeSpectrum(fft);
        }

        /// <summary>
        ///   Obsolete. Please use <see cref="Accord.Math.Transforms.FourierTransform2.GetPowerSpectrum(Complex[])"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use Accord.Math.Transforms.FourierTransform2.GetPowerSpectrum(fft) instead.")]
        public static double[] GetPowerSpectrum(Complex[] fft)
        {
            return Accord.Math.Transforms.FourierTransform2.GetPowerSpectrum(fft);
        }

        /// <summary>
        ///   Obsolete. Please use <see cref="Accord.Math.Transforms.FourierTransform2.GetPhaseSpectrum(Complex[])"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use Accord.Math.Transforms.FourierTransform2.GetPhaseSpectrum(fft) instead.")]
        public static double[] GetPhaseSpectrum(Complex[] fft)
        {
            return Accord.Math.Transforms.FourierTransform2.GetPhaseSpectrum(fft);
        }

        /// <summary>
        ///   Obsolete. Please use <see cref="Accord.Math.Transforms.FourierTransform2.GetFrequencyVector(int, int)"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use Accord.Math.Transforms.FourierTransform2.GetFrequencyVector(length, sampleRate) instead.")]
        public static double[] GetFrequencyVector(int length, int sampleRate)
        {
            return Accord.Math.Transforms.FourierTransform2.GetFrequencyVector(length, sampleRate);

        }

        /// <summary>
        ///   Obsolete. Please use <see cref="Accord.Math.Transforms.FourierTransform2.GetSpectralResolution(int, int)"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use Accord.Math.Transforms.FourierTransform2.GetSpectralResolution(samplingRate, samples) instead.")]
        public static double GetSpectralResolution(int samplingRate, int samples)
        {
            return Accord.Math.Transforms.FourierTransform2.GetSpectralResolution(samplingRate, samples);
        }

        /// <summary>
        ///   Obsolete. Please use <see cref="Accord.Math.Transforms.FourierTransform2.GetPowerCepstrum(Complex[])"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use Accord.Math.Transforms.FourierTransform2.GetPowerCepstrum(signal) instead.")]
        public static double[] GetPowerCepstrum(Complex[] signal)
        {
            return Accord.Math.Transforms.FourierTransform2.GetPowerCepstrum(signal);
        }

        /// <summary>
        ///   Computes the Root-Mean-Square (RMS) value of the given samples.
        /// </summary>
        /// 
        /// <param name="samples">The samples.</param>
        /// 
        /// <returns>The root-mean-square value of the samples.</returns>
        /// 
        public static double RootMeanSquare(this float[] samples)
        {
            return RootMeanSquare(samples, 0, samples.Length);
        }

        /// <summary>
        ///   Computes the Root-Mean-Square (RMS) value of the given samples.
        /// </summary>
        /// 
        /// <param name="samples">The samples.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of samples, starting at start index, to compute.</param>
        /// 
        /// <returns>The root-mean-square value of the samples.</returns>
        /// 
        public static double RootMeanSquare(this float[] samples, int startIndex, int count)
        {
            float sum = 0;
            for (int i = 0; i < count; i++)
            {
                float s = samples[startIndex + i];
                sum += s * s;
            }

            return Math.Sqrt(sum);
        }

        /// <summary>
        ///   Computes the maximum value of the given samples.
        /// </summary>
        /// 
        /// <param name="samples">The samples.</param>
        /// 
        /// <returns>The maximum value of the samples</returns>
        /// 
        public static float Max(this float[] samples)
        {
            return Max(samples, 0, samples.Length);
        }

        /// <summary>
        ///   Computes the maximum value of the given samples.
        /// </summary>
        /// 
        /// <param name="samples">The samples.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of samples, starting at start index, to compute.</param>
        /// 
        /// <returns>The maximum value of the samples</returns>
        /// 
        public static float Max(this float[] samples, int startIndex, int count)
        {
            float max = 0;
            for (int i = 0; i < count; i++)
            {
                if (samples[i + startIndex] > max)
                    max = samples[i];
            }

            return max;
        }

        /// <summary>
        ///   Finds the peaks of a signal.
        /// </summary>
        /// 
        /// <param name="samples">The samples.</param>
        /// 
        /// <returns>The index of the peaks found in the sample.</returns>
        /// 
        public static int[] FindPeaks(this double[] samples)
        {
            var peaks = new List<int>();

            for (int i = 1; i < samples.Length - 1; i++)
            {
                if (samples[i] > samples[i - 1] && samples[i] > samples[i + 1])
                    peaks.Add(i);
            }

            return peaks.ToArray();
        }

        /// <summary>
        ///   Finds the peaks of a signal.
        /// </summary>
        /// 
        /// <param name="samples">The samples.</param>
        /// 
        /// <returns>The index of the peaks found in the sample.</returns>
        /// 
        public static int[] FindPeaks(this float[] samples)
        {
            var peaks = new List<int>();

            for (int i = 1; i < samples.Length - 1; i++)
            {
                if (samples[i] > samples[i - 1] && samples[i] > samples[i + 1])
                    peaks.Add(i);
            }

            return peaks.ToArray();
        }

        /// <summary>
        ///   Obsolete. Please use the extension method from the Accord namespace instead.
        /// </summary>
        /// 
        [Obsolete("Please use the extension method from the Accord namespace instead.")]
        public static byte[] ToByteArray<T>(T value)
            where T : struct
        {
            return ExtensionMethods.ToByteArray(value);
        }

        /// <summary>
        ///   Obsolete. Please use <see cref="ExtensionMethods.ToStruct{T}(byte[], int)"/>
        /// </summary>
        /// 
        [Obsolete("Please use ToStruct<T>() instead.")]
        public static T RawDeserialize<T>(this byte[] rawData)
            where T : struct
        {
            return rawData.ToStruct<T>();
        }

        /// <summary>
        ///   Obsolete. Please use <see cref="ExtensionMethods.ToStruct{T}(byte[], int)"/>
        /// </summary>
        /// 
        [Obsolete("Please use ToStruct<T>() instead.")]
        public static T RawDeserialize<T>(this byte[] rawData, int position)
            where T : struct
        {
            return rawData.ToStruct<T>(position);
        }
    }
}
