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

namespace Accord.Audio.Windows
{

    /// <summary>
    ///   Raised Cosine Window.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The "raised cosine" window is a family of temporal windows, from which
    ///   the most known representative members are the Hann and Hamming windows.</para>
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Window_function">
    ///       Wikipedia, The Free Encyclopedia. Window function. Available on:
    ///       http://en.wikipedia.org/wiki/Window_function </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    public class RaisedCosineWindow : WindowBase
    {
        /// <summary>
        ///   Constructs a new Raised Cosine Window
        /// </summary>
        /// 
        public RaisedCosineWindow(double alpha, double duration, int sampleRate)
            : this(alpha, (int)duration * sampleRate, sampleRate)
        {
        }

        /// <summary>
        ///   Constructs a new Raised Cosine Window
        /// </summary>
        /// 
        public RaisedCosineWindow(double alpha, int length)
            : this(alpha, length,0)
        {
        }

        /// <summary>
        ///   Constructs a new Raised Cosine Window
        /// </summary>
        /// 
        public RaisedCosineWindow(double alpha, int length, int sampleRate)
            : base(length, sampleRate)
        {

            double beta = 1 - alpha;

            for (int i = 0; i < length; i++)
                this[i] = (float)(beta - (alpha * System.Math.Cos((2 * System.Math.PI * i) / (length - 1))));
        }


        /// <summary>
        ///   Creates a new Hamming Window.
        /// </summary>
        /// 
        public static RaisedCosineWindow Hamming(int length)
        {
            return new RaisedCosineWindow(0.46, length);
        }

        /// <summary>
        ///   Creates a new Hann Window.
        /// </summary>
        /// 
        public static RaisedCosineWindow Hann(int length)
        {
            return new RaisedCosineWindow(0.5, length);
        }

        /// <summary>
        ///   Creates a new Hann Window.
        /// </summary>
        /// 
        public static RaisedCosineWindow Hann(double length, int sampleRate)
        {
            return new RaisedCosineWindow(0.5, length, sampleRate);
        }

        /// <summary>
        ///   Creates a new Rectangular Window.
        /// </summary>
        /// 
        /// <param name="length">The size of the window.</param>
        /// 
        public static RaisedCosineWindow Rectangular(int length)
        {
            return new RaisedCosineWindow(0, length);
        }

    }
}
