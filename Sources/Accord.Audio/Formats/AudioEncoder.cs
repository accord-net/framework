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
    using System.Threading;
    using System.Threading.Tasks;
    using Accord.Compat;

    /// <summary>
    /// Audio encoder to encode different custom audio file formats.
    /// </summary>
    /// 
    /// <remarks><para>The class represent a help class, which simplifies encoding of audio
    /// files finding appropriate audio encoder automatically (using list of registered
    /// audio encoders). Instead of using required audio encoder directly, users may use this
    /// class, which will find required encoder by file's extension.</para>
    /// 
    /// <para>
    ///   By default the class will query all referenced assemblies for types that are marked
    ///   with the <see cref="FormatEncoderAttribute"/>. If the user would like to implement
    ///   a new encoder, all that is necessary is to mark a new class with the <see cref="FormatEncoderAttribute"/>
    ///   and make it implement the <see cref="IAudioEncoder"/> interface.</para>
    /// </remarks>
    /// 
    public class AudioEncoder
    {
        private static Dictionary<string, Type> encoderTypes = new Dictionary<string, Type>();
        private static ThreadLocal<Dictionary<string, IAudioEncoder>> encoders = 
            new ThreadLocal<Dictionary<string, IAudioEncoder>>(() => new Dictionary<string, IAudioEncoder>());

        /// <summary>
        /// Encodes a signal from the specified file.
        /// </summary>
        /// 
        /// <param name="fileName">File name to save the signal to.</param>
        /// <param name="signal">The audio signal that should be saved to disk.</param>
        /// 
        public static void EncodeToFile(string fileName, Signal signal)
        {
            string fileExtension = FormatHandlerAttribute.GetNormalizedExtension(fileName);

            IAudioEncoder encoder = FormatEncoderAttribute.GetEncoder(fileExtension, encoderTypes, encoders.Value);

            if (encoder != null)
            {
                // open stream
                using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    // open decoder
                    encoder.Open(stream);

                    // write all audio frames
                    encoder.Encode(signal);

                    encoder.Close();
                }

                return;
            }

            throw new ArgumentException(String.Format("No suitable encoder has been found for the file format {0}. If ", fileExtension) +
                "you are trying to encode .wav files, please add a reference to Accord.Audio.DirectSound, and make sure you " +
                "are using at least one type from assembly in your code (to make sure the assembly is loaded).", "fileName");
        }

    }
}
