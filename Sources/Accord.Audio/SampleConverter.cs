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

// Copyright © Phil Burk, Ross Bencina 1999-2002 
//  Based on the PortAudio conversion mechanism, released under a MIT style license.
//
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files
// (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR
// ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace Accord.Audio
{
    using System;

    /// <summary>
    ///   Static methods to convert between different sample formats.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Code is mainly based on information available on the original
    ///   C source code pa_converters.c from Portable Audio I/O Library.</para>
    /// <para>
    ///   This class try to be as fast as possible without using unsafe code.</para>
    ///   
    /// <para>
    ///   Dither isn't currently supported. Currently supported conversions
    ///   are 'to' and 'from' conversions between the following most common
    ///   PCM format:</para>
    /// <para>
    ///   <list type="bullet">
    ///     <item><description>Integer 8-bit (byte)</description></item>
    ///     <item><description>Integer 16-bit (Int16)</description></item>
    ///     <item><description>Integer 32-bit (Int32)</description></item>
    ///     <item><description>Single precision 32-bit floating point (float)</description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    ///   To use it, just call Convert. The compiler will automatically detect
    ///   which method to call based on your data types.
    ///   
    ///   <code>
    ///   // Suppose we have a collection of samples in PCM-16 format
    ///   // and wish to convert it into IEEE-32 floating point format:
    ///   
    ///   int[]   pcm16Samples = new int  [3] { 1, 2, 3 }; // source
    ///   float[] floatSamples = new float[3];             // target
    /// 
    ///   // Call convert passing the source samples. Converted
    ///   // IEEE samples in will be stored in the target array.
    ///   SampleConverter.Convert(pcm16samples, floatSamples);
    ///   </code>
    /// </example>
    /// 
    public static class SampleConverter
    {

        #region Consts: Standard values used in almost all conversions.
        private const float const_1_div_128_ = 1f / 128f;  // 8 bit multiplier
        private const float const_1_div_32768_ = 1f / 32768f; // 16 bit multiplier
        private const double const_1_div_2147483648_ = 1.0 / 2147483648.0; // 32 bit
        #endregion


        #region From UInt8 (byte)

        #region From UInt8 (byte) to Int16 (short)
        /// <summary>
        ///   Converts a matrix of unsigned 8-bit byte samples
        ///   into a array of 16-bit short integer samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(byte[][] from, Int16[][] to)
        {
            for (int i = 0; i < from.Length; i++)
                for (int j = 0; j < from[0].Length; j++)
                    to[i][j] = (Int16)((from[i][j] - 128) << 8);
        }

        /// <summary>
        ///   Converts an array of unsigned 8-bit byte samples
        ///   into an array of 16-bit short integer samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(byte[] from, Int16[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = (Int16)((from[i] - 128) << 8);
        }

        /// <summary>
        ///   Converts a unsigned 8-bit byte sample
        ///   into a 16-bit short integer sample.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(byte from, out Int16 to)
        {
            to = (Int16)((from - 128) << 8);
        }
        #endregion

        #region From UInt8 (byte) to Int32 (int)
        /// <summary>
        ///   Converts a matrix of unsigned 8-bit byte samples
        ///   into a matrix of 32-bit integer samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(byte[][] from, int[][] to)
        {
            for (int i = 0; i < from.Length; i++)
                for (int j = 0; j < from[0].Length; j++)
                    to[i][j] = ((from[i][j] - 128) << 24);
        }

        /// <summary>
        ///   Converts an array of unsigned 8-bit byte samples
        ///   into an array of 32-bit integer samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(byte[] from, int[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = ((from[i] - 128) << 24);
        }

        /// <summary>
        ///   Converts a unsigned 8-bit byte sample
        ///   into a 32-bit integer sample.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(byte from, out int to)
        {
            to = ((from - 128) << 24);
        }
        #endregion

        #region From UInt8 (byte) to Single (float)
        /// <summary>
        ///   Converts a matrix of unsigned 8-bit byte samples
        ///   into a matrix of 32-bit floating-point samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(byte[][] from, float[][] to)
        {
            for (int i = 0; i < from.Length; i++)
                for (int j = 0; j < from[0].Length; j++)
                    to[i][j] = (from[i][j] - 128) * const_1_div_128_;
        }

        /// <summary>
        ///   Converts an array of unsigned 8-bit byte samples
        ///   into an array of 32-bit floating point samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(byte[] from, float[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = (from[i] - 128) * const_1_div_128_;
        }

        /// <summary>
        ///   Converts a unsigned 8-bit byte sample
        ///   into a 32-bit floating point sample.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(byte from, out float to)
        {
            to = (from - 128) * const_1_div_128_;
        }
        #endregion

        #endregion


        #region From Int16 (short)

        #region From Int16 (short) to UInt8 (byte)
        /// <summary>
        ///   Converts a matrix of signed 16-bit integer samples
        ///   into a matrix of 8-bit unsigned byte samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(Int16[][] from, byte[][] to)
        {
            for (int j = 0; j < from.Length; j++)
                for (int i = 0; i < from[0].Length; i++)
                    to[i][j] = (byte)(((from[i][j]) >> 8) + 128);
        }

        /// <summary>
        ///   Converts a array of signed 16-bit integer samples
        ///   into a array of 8-bit unsigned byte samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(Int16[] from, byte[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = (byte)(((from[i]) >> 8) + 128);
        }

        /// <summary>
        ///   Converts a signed 16-bit integer sample
        ///   into a 8-bit unsigned byte sample.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(Int16 from, out byte to)
        {
            to = (byte)(((from) >> 8) + 128);
        }
        #endregion

        #region From Int16 (short) to Int32 (int)
        /// <summary>
        ///   Converts a matrix of signed 16-bit integer samples
        ///   into a matrix of 32-bit signed integer samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(Int16[][] from, Int32[][] to)
        {
            for (int i = 0; i < from.Length; i++)
                for (int j = 0; j < from[0].Length; j++)
                    to[i][j] = (byte)(((from[i][j]) >> 8) + 128);
        }

        /// <summary>
        ///   Converts a array of signed 16-bit integer samples
        ///   into a array of 32-bit signed integer samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(Int16[] from, Int32[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = (byte)(((from[i]) >> 8) + 128);
        }

        /// <summary>
        ///   Converts a signed 16-bit integer sample
        ///   into a 32-bit signed integer sample.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(Int16 from, out Int32 to)
        {
            to = (byte)(((from) >> 8) + 128);
        }
        #endregion

        #region From Int16 (short) to Single (float)
        /// <summary>
        ///   Converts a matrix of signed 16-bit integer samples
        ///   into a matrix of 32-bit floating point samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(Int16[][] from, float[][] to)
        {
            for (int i = 0; i < from.Length; i++)
                for (int j = 0; j < from[0].Length; j++)
                    to[i][j] = (float)(from[i][j] * const_1_div_32768_);
        }

        /// <summary>
        ///   Converts a array of signed 16-bit integer samples
        ///   into a array of 32-bit floating point samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(Int16[] from, float[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = (float)(from[i] * const_1_div_32768_);
        }

        /// <summary>
        ///   Converts a signed 16-bit integer sample
        ///   into a 32-bit floating point sample.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(Int16 from, out float to)
        {
            to = (float)(from * const_1_div_32768_);
        }
        #endregion

        #region From Int16 (short) to Single (float)
        /// <summary>
        ///   Converts a matrix of signed 16-bit integer samples
        ///   into a matrix of 64-bit floating point samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(Int16[][] from, double[][] to)
        {
            for (int i = 0; i < from.Length; i++)
                for (int j = 0; j < from[0].Length; j++)
                    to[i][j] = (double)(from[i][j] * const_1_div_32768_);
        }

        /// <summary>
        ///   Converts a array of signed 16-bit integer samples
        ///   into a array of 64-bit floating point samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(Int16[] from, double[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = (double)(from[i] * const_1_div_32768_);
        }

        /// <summary>
        ///   Converts a signed 16-bit integer sample
        ///   into a 64-bit floating point sample.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(Int16 from, out double to)
        {
            to = (double)(from * const_1_div_32768_);
        }
        #endregion

        #endregion


        #region From Int32 (int)

        #region From Int32 (int) To UInt8 (byte)
        /// <summary>
        ///   Converts a matrix of signed 32-bit integer samples
        ///   into a matrix of 8-bit unsigned byte samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(Int32[][] from, byte[][] to)
        {
            for (int i = 0; i < from.Length; i++)
                for (int j = 0; j < from[0].Length; j++)
                    to[i][j] = (byte)(((from[i][j]) >> 24) + 128);
        }

        /// <summary>
        ///   Converts a array of signed 32-bit integer samples
        ///   into a array of 8-bit unsigned byte samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(Int32[] from, byte[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = (byte)(((from[i]) >> 24) + 128);
        }

        /// <summary>
        ///   Converts a signed 32-bit integer sample
        ///   into a 8-bit unsigned byte sample.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(Int32 from, out byte to)
        {
            to = (byte)((from >> 24) + 128);
        }
        #endregion

        #region From Int32 (int) to Int16 (short)
        /// <summary>
        ///   Converts a matrix of signed 32-bit integer samples
        ///   into a matrix of 16-bit signed integer samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(Int32[][] from, Int16[][] to)
        {
            for (int i = 0; i < from.Length; i++)
                for (int j = 0; j < from[0].Length; j++)
                    to[i][j] = (Int16)(from[i][j] >> 16);
        }

        /// <summary>
        ///   Converts a array of signed 32-bit integer samples
        ///   into a array of 16-bit signed integer samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(Int32[] from, Int16[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = (Int16)(from[i] >> 16);
        }

        /// <summary>
        ///   Converts a signed 32-bit integer sample
        ///   into a 16-bit signed integer sample.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(Int32 from, out Int16 to)
        {
            to = (Int16)(from >> 16);
        }
        #endregion

        #region From Int32 (int) to Single (float)
        /// <summary>
        ///   Converts a matrix of signed 32-bit integer samples
        ///   into a matrix of 32-bit float-point samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(Int32[][] from, float[][] to)
        {
            for (int j = 0; j < from.Length; j++)
                for (int i = 0; i < from[i].Length; i++)
                    to[i][j] = (float)(from[i][j] * const_1_div_2147483648_);
        }

        /// <summary>
        ///   Converts a array of signed 32-bit integer samples
        ///   into a array of 32-bit float-point samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(Int32[] from, float[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = (float)((double)from[i] * const_1_div_2147483648_);
        }

        /// <summary>
        ///   Converts a signed 32-bit integer sample
        ///   into a 32-bit float-point sample.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(Int32 from, out float to)
        {
            to = (float)((double)from * const_1_div_2147483648_);
        }
        #endregion

        #endregion


        #region From Single (float)

        #region From Single (float) to UInt8 (byte)
        /// <summary>
        ///   Converts a matrix of signed 32-bit float samples
        ///   into a matrix of 8-bit unsigned byte samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(float[][] from, byte[][] to)
        {
            for (int i = 0; i < from.Length; i++)
                for (int j = 0; j < from[0].Length; j++)
                    to[i][j] = (byte)(128 + ((byte)(to[i][j] * (127f))));
        }

        /// <summary>
        ///   Converts a array of signed 32-bit float samples
        ///   into a array of 8-bit unsigned byte samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(float[] from, byte[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = (byte)(128 + ((byte)(to[i] * (127f))));
        }

        /// <summary>
        ///   Converts a signed 32-bit float sample
        ///   into a 8-bit unsigned byte sample.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(float from, out byte to)
        {
            to = (byte)(128 + ((byte)(from * (127f))));
        }
        #endregion

        #region From Single (float) to Int16 (short)
        /// <summary>
        ///   Converts a matrix of 32-bit float samples
        ///   into a matrix of 16-bit integer samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(float[][] from, short[][] to)
        {
            for (int i = 0; i < from.Length; i++)
                for (int j = 0; i < from[0].Length; j++)
                    to[i][j] = (short)(from[i][j] * (32767f));
        }

        /// <summary>
        ///   Converts a array of 32-bit float samples
        ///   into a array of 16-bit integer samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(float[] from, short[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = (short)(from[i] * (32767f));
        }

        /// <summary>
        ///   Converts a 32-bit float sample
        ///   into a 16-bit integer sample.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(float from, out short to)
        {
            to = (short)(from * (32767f));
        }
        #endregion

        #region From Single (float) to Int32 (int)
        /// <summary>
        ///   Converts a matrix of signed 32-bit integer samples
        ///   into a matrix of 32-bit floating point samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(float[][] from, Int32[][] to)
        {
            for (int i = 0; i < from.Length; i++)
                for (int j = 0; j < from[0].Length; j++)
                    to[i][j] = (int)((double)from[i][j] * 0x7FFFFFFF);
        }

        /// <summary>
        ///   Converts a array of signed 32-bit integer samples
        ///   into a array of 32-bit floating point samples.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(Int32[] from, Int32[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = (int)((double)from[i] * 0x7FFFFFFF);
        }

        /// <summary>
        ///   Converts a signed 32-bit integer sample
        ///   into a 32-bit floating point sample.
        /// </summary>
        /// 
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        /// 
        public static void Convert(Int32 from, out Int32 to)
        {
            to = (int)((double)from * 0x7FFFFFFF);
        }
        #endregion

        #endregion

    }
}