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

namespace Accord.Audio.Formats
{
    using System;
    using System.IO;
    using Accord.Audio;

    /// <summary>
    ///   Audio decoder interface, which specifies set of methods that should
    ///   be implemented by audio decoders for different file formats.
    /// </summary>
    /// 
    public interface IAudioDecoder
    {

        /// <summary>
        ///   Open specified stream.
        /// </summary>
        /// 
        /// <param name="stream">Stream to open.</param>
        /// 
        /// <returns>Returns number of frames found in the specified stream.</returns>
        /// 
        /// <remarks><para>Implementation of this method is supposed to read audio's header,
        /// checking for correct audio format and reading its attributes.</para>
        /// 
        /// <para>Implementations of this method may throw
        /// <see cref="System.FormatException"/> exception to report about unrecognized audio
        /// format, <see cref="System.ArgumentException"/> exception to report about incorrectly
        /// formatted audio or <see cref="NotSupportedException"/> exception to report if
        /// certain formats are not supported.</para>
        /// </remarks>
        /// 
        int Open(Stream stream);

        /// <summary>
        ///   Decode all frames.
        /// </summary>
        /// 
        /// <returns>Returns the decoded signal.</returns>
        /// 
        /// <remarks>Implementations of this method may throw
        /// <see cref="System.NullReferenceException"/> exception in the case if no audio
        /// stream was opened previously, <see cref="System.ArgumentOutOfRangeException"/> in the
        /// case if stream does not contain frame with specified index or  <see cref="System.ArgumentException"/>
        /// exception to report about incorrectly formatted audio.
        /// </remarks>
        /// 
        Signal Decode();

        /// <summary>
        ///   Decode a number of frames.
        /// </summary>
        /// 
        /// <param name="index">Audio frame index to start decoding.</param>
        /// <param name="frames">The number of frames to decode.</param>
        /// 
        /// <returns>Returns the decoded signal.</returns>
        /// 
        /// <remarks>Implementations of this method may throw
        /// <see cref="System.NullReferenceException"/> exception in the case if no audio
        /// stream was opened previously, <see cref="System.ArgumentOutOfRangeException"/> in the
        /// case if stream does not contain frame with specified index or  <see cref="System.ArgumentException"/>
        /// exception to report about incorrectly formatted audio.
        /// </remarks>
        /// 
        Signal Decode(int index, int frames);


        /// <summary>
        ///   Close decoding of previously opened stream.
        /// </summary>
        /// 
        /// <remarks><para>Implementations of this method don't close stream itself, but just close
        /// decoding cleaning all associated data with it.</para></remarks>
        /// 
        void Close();
    }
}
