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

namespace Accord.Audio.Formats
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Audio decoder to decode different custom audio file formats.
    /// </summary>
    /// 
    /// <remarks><para>The class represent a help class, which simplifies decoding of audio
    /// files finding appropriate image decoder automatically (using list of registered
    /// audio decoders). Instead of using required audio decoder directly, users may use this
    /// class, which will find required decoder by file's extension.</para>
    /// 
    /// <para>
    ///   By default the class will query all referenced assemblies for types that are marked
    ///   with the <see cref="FormatDecoderAttribute"/>. If the user would like to implement
    ///   a new decoder, all that is necessary is to mark a new class with the <see cref="FormatDecoderAttribute"/>
    ///   and make it implement the <see cref="IAudioDecoder"/> interface.</para>
    /// </remarks>
    /// 
    /// <example>
    /// <code source="Unit Tests\Accord.Tests.Imaging\Formats\PNMCodecTest.cs" region="doc_load" />
    /// </example>
    /// 
    public class AudioDecoder
    {
        private static Dictionary<string, Type> decoders = new Dictionary<string, Type>();

        /// <summary>
        /// Decode a signal from the specified file.
        /// </summary>
        /// 
        /// <param name="fileName">File name to read signal from.</param>
        /// 
        /// <returns>Return decoded signal.</returns>
        /// 
        public static Signal DecodeFromFile(string fileName)
        {
            FrameInfo info;
            return DecodeFromFile(fileName, out info);
        }

        /// <summary>
        /// Decode a signal from the specified file.
        /// </summary>
        /// 
        /// <param name="fileName">File name to read signal from.</param>
        /// <param name="frameInfo">Information about the decoded signal.</param>
        /// 
        /// <returns>Return decoded signal.</returns>
        /// 
        public static Signal DecodeFromFile(string fileName, out FrameInfo frameInfo)
        {
            Signal signal = null;

            string fileExtension = Path.GetExtension(fileName).ToUpperInvariant();

            if ((fileExtension != string.Empty) && (fileExtension.Length != 0))
            {
                fileExtension = fileExtension.Substring(1);

                if (!decoders.ContainsKey(fileExtension))
                    FormatDecoderAttribute.PopulateDictionaryWithDecodersFromAllAssemblies<IAudioDecoder>(decoders, fileExtension);

                if (decoders.ContainsKey(fileExtension))
                {
                    IAudioDecoder decoder = (IAudioDecoder)Activator.CreateInstance(decoders[fileExtension]);

                    // open stream
                    using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        // open decoder
                        decoder.Open(stream);

                        // read all audio frames
                        signal = decoder.Decode();

                        decoder.Close();
                    }

                    frameInfo = new FrameInfo(signal.Channels, signal.SampleRate, Signal.GetSampleSize(signal.SampleFormat), 0, signal.Length);

                    return signal;
                }
            }

            throw new ArgumentException(String.Format("No suitable decoder has been found for the file format {0}. If ", fileExtension) +
                "you are trying to decode .wav files, please add a reference to Accord.Audio.DirectSouond.", "fileName");
        }

    }
}
