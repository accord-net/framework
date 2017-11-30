// Accord Audio Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// Copyright © Yang Ting, 2017
// skype.samhong at yahoo.com
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
// Based on the original MFCC code from the Sphinx-3 project:
//
//    Copyright(c) 1999-2004 Carnegie Mellon University. All rights reserved.
//
//    Redistribution and use in source and binary forms, with or without
//    modification, are permitted provided that the following conditions
//    are met:
//    
//      1. Redistributions of source code must retain the above copyright
//      notice, this list of conditions and the following disclaimer.
//      
//      2. Redistributions in binary form must reproduce the above copyright
//      notice, this list of conditions and the following disclaimer in
//         the documentation and/or other materials provided with the
//         distribution.
//    
//    This work was supported in part by funding from the Defense Advanced
//    Research Projects Agency and the National Science Foundation of the
//    United States of America, and the CMU Sphinx Speech Consortium.
//    
//    THIS SOFTWARE IS PROVIDED BY CARNEGIE MELLON UNIVERSITY ``AS IS'' AND
//    ANY EXPRESSED OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
//    THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
//    PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL CARNEGIE MELLON UNIVERSITY
//    NOR ITS EMPLOYEES BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
//    SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
//    LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
//    DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
//    THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//    (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
//    OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//

namespace Accord.Audio
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Numerics;
    using System.Threading.Tasks;
    using Accord.Audio;
    using Accord.Audio.Windows;
    using Accord.Math;
    using Accord.Compat;
    using Accord.Audio.Filters;

    /// <summary>
    ///   Obsolete. Please use <see cref="MelFrequencyCepstrumCoefficient"/> instead.
    /// </summary>
    /// 
    [Obsolete("Please use MelFrequencyCepstrumCoefficient instead.")]
    [Serializable]
    public class MFCC : MelFrequencyCepstrumCoefficient
    {
        /// <summary>
        ///   Obsolete. Please use <see cref="MelFrequencyCepstrumCoefficient"/> instead.
        /// </summary>
        /// 
        public MFCC(
            int filterCount = 40,
            int cepstrumCount = 13,
            double lowerFrequency = 133.3333,
            double upperFrequency = 6855.4976,
            double alpha = 0.97,
            int samplingRate = 16000,
            int frameRate = 100,
            double windowLength = 0.0256,
            int numberOfBins = 512
            )
            : base(filterCount, cepstrumCount, lowerFrequency, upperFrequency, alpha, samplingRate, frameRate, windowLength, numberOfBins)
        {
        }

        /// <summary>
        ///   Extracts MFCC feature vectors from an audio <see cref="Signal"/>.
        /// </summary>
        /// 
        /// <param name="signal">The signal to be processed.</param>
        /// 
        public double[][] ProcessSignal(Signal signal)
        {
            return Transform(signal).Select(x => x.Descriptor).ToArray();
        }
    }

    /// <summary>
    ///   Mel-Frequency Cepstral Coefficients.
    /// </summary>
    /// 
    /// <example>
    /// <code source="Unit Tests\Accord.Tests.Audio\MFCCTests.cs" region="doc_example1" />
    /// </example>
    /// 
    [Serializable]
    public class MelFrequencyCepstrumCoefficient : BaseAudioFeatureExtractor<MelFrequencyCepstrumCoefficientDescriptor>
    {
        private int m_nfilt;
        private int m_ncep;
        private double m_lowerf;
        private double m_upperf;
        private double m_alpha;
        private int m_frate;
        private double m_windowLength;
        private int m_wlen;
        private int m_nfft;
        private float m_fshift;
        private int m_samplingRate;
        private RaisedCosineWindow m_win;
        private int m_prior;
        private double[,] m_filters;
        private double[,] m_s2dct;
        private double[,] m_dst;

        /// <summary>
        /// Initializes a new instance of the <see cref="MFCC" /> class.
        /// </summary>
        /// 
        /// <param name="filterCount">The filter count.</param>
        /// <param name="cepstrumCount">The cepstrum count.</param>
        /// <param name="lowerFrequency">The lower frequency.</param>
        /// <param name="upperFrequency">The upper frequency.</param>
        /// <param name="alpha">The alpha.</param>
        /// <param name="samplingRate">The sampling rate.</param>
        /// <param name="frameRate">The frame rate.</param>
        /// <param name="windowLength">Length of the window.</param>
        /// <param name="numberOfBins">The number of bins.</param>
        /// 
        public MelFrequencyCepstrumCoefficient(
            int filterCount = 40,
            int cepstrumCount = 13,
            double lowerFrequency = 133.3333,
            double upperFrequency = 6855.4976,
            double alpha = 0.97,
            int samplingRate = 16000,
            int frameRate = 100,
            double windowLength = 0.0256,
            int numberOfBins = 512
            )
        {
            base.SupportedFormats.UnionWith(new[]
            {
                SampleFormat.Format32BitIeeeFloat
            });

            m_lowerf = lowerFrequency;
            m_upperf = upperFrequency;
            m_nfft = numberOfBins;
            m_ncep = cepstrumCount;
            m_nfilt = filterCount;
            m_frate = frameRate;
            m_fshift = (float)samplingRate / frameRate;
            m_samplingRate = samplingRate;
            m_windowLength = windowLength;

            // Build Hamming window
            m_wlen = (int)(windowLength * samplingRate);
            m_win = RaisedCosineWindow.Hamming(m_wlen);

            // Prior sample for pre-emphasis
            m_prior = 0;
            m_alpha = alpha;

            // Build mel filter matrix
            m_filters = new double[numberOfBins / 2 + 1, filterCount];

            float w_dfreq = (float)samplingRate / (float)numberOfBins;

            if (upperFrequency > samplingRate / 2)
                throw new Exception("Upper frequency exceeds Nyquist");

            double w_melmax = mel(upperFrequency);
            double w_melmin = mel(lowerFrequency);
            double w_dmelbw = (w_melmax - w_melmin) / (filterCount + 1);

            // Filter edges, in Hz
            double[] w_filt_edge = new double[filterCount + 2];
            for (double w_i = 0; w_i < filterCount + 2; w_i += 1.0)
                w_filt_edge[(int)w_i] = w_melmin + w_dmelbw * w_i;

            w_filt_edge = melinv(w_filt_edge);

            for (int w_whichfilt = 0; w_whichfilt < filterCount; w_whichfilt++)
            {
                // Filter triangles, in DFT points
                int w_leftfr = (int)System.Math.Round(w_filt_edge[w_whichfilt] / w_dfreq);
                int w_centerfr = (int)System.Math.Round(w_filt_edge[w_whichfilt + 1] / w_dfreq);
                int w_rightfr = (int)System.Math.Round(w_filt_edge[w_whichfilt + 2] / w_dfreq);

                // For some reason this is calculated in Hz, though I think it doesn't really matter
                double w_fwidth = (w_rightfr - w_leftfr) * w_dfreq;
                double w_height = 2.0 / w_fwidth;

                double w_leftslope = 0;
                if (w_centerfr != w_leftfr)
                    w_leftslope = w_height / (w_centerfr - w_leftfr);

                int w_freq = w_leftfr + 1;
                while (w_freq < w_centerfr)
                {
                    m_filters[w_freq, w_whichfilt] = (w_freq - w_leftfr) * w_leftslope;
                    w_freq = w_freq + 1;
                }

                if (w_freq == w_centerfr) // This is always true
                {
                    m_filters[w_freq, w_whichfilt] = w_height;
                    w_freq = w_freq + 1;
                }
                double w_rightslope = 0;
                if (w_centerfr != w_rightfr)
                {
                    w_rightslope = w_height / (w_centerfr - w_rightfr);
                }

                while (w_freq < w_rightfr)
                {
                    m_filters[w_freq, w_whichfilt] = (w_freq - w_rightfr) * w_rightslope;
                    w_freq = w_freq + 1;
                }
            }
            // Build DCT matrix
            m_s2dct = s2dctmat(filterCount, cepstrumCount, 1.0 / filterCount);
            m_dst = dctmat(filterCount, cepstrumCount, System.Math.PI / filterCount);
        }


        /// <summary>
        ///   This method should be implemented by inheriting classes to implement the 
        ///   actual feature extraction, transforming the input image into a list of features.
        /// </summary>
        /// 
        protected override IList<MelFrequencyCepstrumCoefficientDescriptor> InnerTransform(Signal signal)
        {
            if (signal.NumberOfChannels > 1)
            {
                signal = new MonoFilter().Apply(signal);
            }

            float[] w_fSig = new float[signal.Length];
            signal.CopyTo(w_fSig);

            int w_nfr = (int)(signal.Length / m_fshift + 1);
            var w_mfcc = new MelFrequencyCepstrumCoefficientDescriptor[w_nfr];
            for (int w_fr = 0; w_fr < w_nfr; w_fr++)
            {
                int w_start = (int)System.Math.Round(w_fr * m_fshift);
                int w_end = System.Math.Min(signal.Length, w_start + m_wlen);

                Int16[] w_frame = new Int16[w_end - w_start];
                int w_j = 0;
                for (int w_i = w_start; w_i < w_end; w_i++, w_j++)
                    w_frame[w_j] = (Int16)(w_fSig[w_i] * 32768f);

                int w_len = w_frame.Length;
                if (w_len < m_wlen)
                {
                    Array.Resize(ref w_frame, m_wlen);
                    for (int w_i = w_len; w_i < m_wlen; w_i++)
                        w_frame[w_i] = 0;
                }

                double[] w_s2mfc = frame2s2mfc(w_frame);
                w_mfcc[w_fr] = new MelFrequencyCepstrumCoefficientDescriptor(w_fr, w_s2mfc);
            }

            return w_mfcc;
        }


        /// <summary>
        ///   After the speech signals hass been sent to a high-pass filter in the previous steps of the
        ///   algorithm, this method compensates the high-frequency parts that were suppressed during the 
        ///   sound processing mechanism of humans. It can also amplify the importance of high-frequency
        ///   <a href="https://en.wikipedia.org/wiki/Formant">formants</a>.
        /// </summary>
        /// 
        private double[] pre_emphasis(params Int16[] p_frame)
        {
            double[] w_outfr = new double[p_frame.Length];
            w_outfr[0] = (p_frame[0] - m_alpha * m_prior);
            for (int w_i = 1; w_i < p_frame.Length; w_i++)
                w_outfr[w_i] = (p_frame[w_i] - m_alpha * p_frame[w_i - 1]);
            m_prior = p_frame[p_frame.Length - 1];
            return w_outfr;
        }

        /// <summary>
        ///   Converts a frame to log-power-spectrum bins.
        /// </summary>
        /// 
        private double[] frame2logspec(params Int16[] p_frame)
        {
            double[] w_preEmp = pre_emphasis(p_frame);
            double[] w_win = new double[m_win.Length];
            for (int w_i = 0; w_i < m_win.Length; w_i++)
                w_win[w_i] = m_win[w_i];

            // Each frame has to be multiplied with a hamming window in order to keep 
            // the continuity of the first and the last points in the frame.
            double[] w_frame = new double[w_win.Length];
            for (int w_i = 0; w_i < w_win.Length; w_i++)
                w_frame[w_i] = w_preEmp[w_i] * w_win[w_i];

            Complex[] w_complexFrame = new Complex[m_nfft];
            for (int w_i = 0; w_i < w_frame.Length && w_i < m_nfft; w_i++)
                w_complexFrame[w_i] = w_frame[w_i];
            for (int w_i = w_frame.Length; w_i < m_nfft; w_i++)
                w_complexFrame[w_i] = 0;

            // Compute the one-dimensional discrete Fourier Transform for real input.
            FourierTransform.FFT(w_complexFrame, FourierTransform.Direction.Backward);

            // Square of absolute value
            double[] w_power = new double[m_nfft / 2 + 1];
            for (int w_i = 0; w_i < m_nfft / 2 + 1; w_i++)
                w_power[w_i] = w_complexFrame[w_i].Real * w_complexFrame[w_i].Real
                                    + w_complexFrame[w_i].Imaginary * w_complexFrame[w_i].Imaginary;

            double[] w_dotMat = Matrix.Dot(w_power, m_filters);
            double[] w_ret = new double[w_dotMat.Length];
            for (int w_i = 0; w_i < w_ret.Length; w_i++)
            {
                if (w_dotMat[w_i] < 1.0f / 100000.0f)
                    w_ret[w_i] = System.Math.Log(1.0f / 100000.0f);
                else
                    w_ret[w_i] = System.Math.Log(w_dotMat[w_i]);
            }
            return w_ret;
        }

        /// <summary>
        ///   Convert a frame to Sphinx-II feature format.
        /// </summary>
        /// 
        private double[] frame2s2mfc(params Int16[] p_frame)
        {
            double[] w_logspec = frame2logspec(p_frame);
            double[] w_ret = Matrix.Dot(w_logspec, m_s2dct.Transpose());
            for (int w_i = 0; w_i < w_ret.Length; w_i++)
                w_ret[w_i] = w_ret[w_i] / m_nfilt;
            return w_ret;
        }

        /// <summary>
        ///   Return the 'legacy' not-quite-DCT matrix used by Sphinx.
        /// </summary>
        /// 
        private static double[,] s2dctmat(int p_nfilt, int p_ncep, double p_freqstep)
        {
            double[,] w_melcos = new double[p_ncep, p_nfilt];
            for (int w_i = 0; w_i < p_ncep; w_i++)
            {
                double w_freq = System.Math.PI * (float)w_i / p_nfilt;
                double w_k = 0.5;
                for (int w_j = 0; w_j < p_nfilt && w_k < (float)p_nfilt + 0.5; w_j++, w_k += 1.0)
                    w_melcos[w_i, w_j] = System.Math.Cos(w_freq * w_k);
                w_melcos[w_i, 0] *= 0.5;
            }

            return w_melcos;
        }

        /// <summary>
        ///   Convert log-power-spectrum bins to MFCC using the 'legacy' Sphinx transform.
        /// </summary>
        /// 
        internal static double[,] logspec2s2mfc(double[,] p_logspec, int p_ncep = 13)
        {
            int w_nfilt = p_logspec.GetLength(1);
            double[,] w_melcos = s2dctmat(w_nfilt, p_ncep, 1.0f / w_nfilt);
            double[,] w_ret = Matrix.Dot(p_logspec, w_melcos.Transpose());
            for (int w_i = 0; w_i < w_ret.GetLength(0); w_i++)
            {
                for (int w_j = 0; w_j < w_ret.GetLength(1); w_j++)
                    w_ret[w_i, w_j] /= w_nfilt;
            }
            return w_ret;
        }

        /// <summary>
        ///   Returns the orthogonal DCT-II/DCT-III matrix of size p_N x p_K. For computing or inverting 
        ///   MFCCs, p_N is the number of log-power-spectrum bins while p_K is the number of cepstra.
        /// </summary>
        /// 
        private static double[,] dctmat(int p_N, int p_K, double p_freqstep, bool p_orthogonalize = true)
        {
            double[,] w_cosmat = new double[p_N, p_K];
            for (int w_n = 0; w_n < p_N; w_n++)
            {
                for (int w_k = 0; w_k < p_K; w_k++)
                    w_cosmat[w_n, w_k] = System.Math.Cos(p_freqstep * (w_n + 0.5) * w_k);
            }

            if (p_orthogonalize)
            {
                for (int w_n = 0; w_n < p_N; w_n++)
                    w_cosmat[w_n, 0] *= 1.0 / Constants.Sqrt2;
            }
            return w_cosmat;
        }

        /// <summary>
        ///   Convert log-power-spectrum to MFCC using the orthogonal DCT-II
        /// </summary>
        /// 
        internal static double[,] dct(double[,] p_data, int p_K = 13)
        {
            int w_N = p_data.GetLength(1);
            double w_freqstep = System.Math.PI / w_N;
            double[,] w_cosmat = dctmat(w_N, p_K, w_freqstep);
            double[,] w_ret = Matrix.Dot(p_data, w_cosmat);

            for (int w_i = 0; w_i < w_ret.GetLength(0); w_i++)
            {
                for (int w_j = 0; w_j < w_ret.GetLength(1); w_j++)
                    w_ret[w_i, w_j] *= System.Math.Sqrt(2.0f / (float)w_N);
            }

            return w_ret;
        }

        /// <summary>
        ///   Convert log-power-spectrum to MFCC using the normalized DCT-II.
        /// </summary>
        /// 
        internal static double[,] dct2(double[,] p_data, int p_K = 13)
        {
            int w_N = p_data.GetLength(1);
            double w_freqstep = System.Math.PI / w_N;
            double[,] w_cosmat = dctmat(w_N, p_K, w_freqstep, false);
            double[,] w_ret = Matrix.Dot(p_data, w_cosmat);

            for (int w_i = 0; w_i < w_ret.GetLength(0); w_i++)
            {
                for (int w_j = 0; w_j < w_ret.GetLength(1); w_j++)
                    w_ret[w_i, w_j] *= (2.0f / (float)w_N);
            }

            return w_ret;
        }

        /// <summary>
        ///   Convert MFCC to log-power-spectrum using the orthogonal DCT-III.
        /// </summary>
        /// 
        internal static double[,] idct(double[,] p_data, int p_K = 40)
        {
            int w_N = p_data.GetLength(1);
            double w_freqstep = System.Math.PI / p_K;
            double[,] w_cosmat = dctmat(p_K, w_N, w_freqstep);
            double[,] w_cosmatT = w_cosmat.Transpose();
            double[,] w_ret = Matrix.Dot(p_data, w_cosmatT);

            for (int w_i = 0; w_i < w_ret.GetLength(0); w_i++)
            {
                for (int w_j = 0; w_j < w_ret.GetLength(1); w_j++)
                    w_ret[w_i, w_j] *= System.Math.Sqrt(2.0f / (float)p_K);
            }

            return w_ret;
        }

        /// <summary>
        ///   Convert MFCC to log-power-spectrum using the unnormalized DCT-III.
        /// </summary>
        /// 
        internal static double[,] dct3(double[,] p_data, int p_K = 40)
        {
            int w_N = p_data.GetLength(1);
            double w_freqstep = System.Math.PI / p_K;
            double[,] w_cosmat = dctmat(p_K, w_N, w_freqstep, false);
            for (int w_i = 0; w_i < w_cosmat.GetLength(0); w_i++)
                w_cosmat[w_i, 0] *= 0.5f;
            double[,] w_ret = Matrix.Dot(p_data, w_cosmat.Transpose());
            return w_ret;
        }

        /// <summary>
        ///   Calculate Mel-frequency, which is proportional to the logarithm of the linear 
        ///   frequency, reflecting similar effects in the human's subjective aural perception.
        /// </summary>
        /// 
        private static double mel(double p_f)
        {
            return 2595.0 * System.Math.Log10(1.0 + p_f / 700.0);
        }

        private static double[] melinv(params double[] p_m)
        {
            double[] w_ret = p_m;
            for (int w_i = 0; w_i < p_m.Length; w_i++)
                w_ret[w_i] = 700.0 * (System.Math.Pow(10.0, p_m[w_i] / 2595.0) - 1.0);
            return w_ret;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        public override object Clone()
        {
            return new MelFrequencyCepstrumCoefficient(
                filterCount: m_nfilt,
                cepstrumCount: m_ncep,
                lowerFrequency: m_lowerf,
                upperFrequency: m_upperf,
                alpha: m_alpha,
                samplingRate: m_samplingRate,
                frameRate: m_frate,
                windowLength: m_windowLength,
                numberOfBins: m_nfft);
        }

    }
}
