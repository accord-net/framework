// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// AForge Image Formats Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
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

namespace Accord.Imaging.Formats
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Image decoder to decode different custom image file formats.
    /// </summary>
    /// 
    /// <remarks><para>The class represent a help class, which simplifies decoding of image
    /// files finding appropriate image decoder automatically (using list of registered
    /// image decoders). Instead of using required image decoder directly, users may use this
    /// class, which will find required decoder by file's extension.</para>
    /// 
    /// <para>By default the class registers on its own all decoders, which are available in
    /// AForge.Imaging.Formats library. If user has implementation of his own image decoders, he
    /// needs to register them using <see cref="RegisterDecoder"/> method to be able to use them through
    /// the <see cref="ImageDecoder"/> class.</para>
    /// 
    /// <para><note>If the class can not find appropriate decode in the list of registered
    /// decoders, it passes file to .NET's image decoder for decoding.</note></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code source="Unit Tests\Accord.Tests.Imaging\Formats\PNMCodecTest.cs" region="doc_load" />
    /// </example>
    /// 
    /// <seealso cref="PNMCodec"/>
    /// <seealso cref="FITSCodec"/>
    /// 
    public static class ImageDecoder
    {
        private static Dictionary<string, IImageDecoder> decoders = new Dictionary<string, IImageDecoder>();

        static ImageDecoder()
        {
            // register PNM file format
            IImageDecoder decoder = new PNMCodec();

            RegisterDecoder("pbm", decoder);
            RegisterDecoder("pgm", decoder);
            RegisterDecoder("pnm", decoder);
            RegisterDecoder("ppm", decoder);

            // register FITS file format
            decoder = new FITSCodec();

            RegisterDecoder("fit", decoder);
            RegisterDecoder("fits", decoder);
        }

        /// <summary>
        /// Register image decoder for a specified file extension.
        /// </summary>
        /// 
        /// <param name="fileExtension">File extension to register decoder for ("bmp", for example).</param>
        /// <param name="decoder">Image decoder to use for the specified file extension.</param>
        /// 
        /// <remarks><para>The method allows to register image decoder object, which should be used
        /// to decode images from files with the specified extension.</para></remarks>
        /// 
        public static void RegisterDecoder(string fileExtension, IImageDecoder decoder)
        {
            System.Diagnostics.Debug.WriteLine("Registering decoder: " + fileExtension);

            decoders.Add(fileExtension.ToUpperInvariant(), decoder);
        }

        /// <summary>
        /// Decode first frame for the specified file.
        /// </summary>
        /// 
        /// <param name="fileName">File name to read image from.</param>
        /// 
        /// <returns>Return decoded image. In the case if file format support multiple
        /// frames, the method return the first frame.</returns>
        /// 
        /// <remarks><para>The method uses table of registered image decoders to find the one,
        /// which should be used for the specified file. If there is not appropriate decoder
        /// found, the method uses default .NET's image decoding routine (see
        /// <see cref="System.Drawing.Image.FromFile( string )"/>).</para></remarks>
        /// 
        public static Bitmap DecodeFromFile(string fileName)
        {
            ImageInfo imageInfo = null;

            return DecodeFromFile(fileName, out imageInfo);
        }

        /// <summary>
        /// Decode first frame for the specified file.
        /// </summary>
        /// 
        /// <param name="fileName">File name to read image from.</param>
        /// <param name="imageInfo">Information about the decoded image.</param>
        /// 
        /// <returns>Return decoded image. In the case if file format support multiple
        /// frames, the method return the first frame.</returns>
        /// 
        /// <remarks><para>The method uses table of registered image decoders to find the one,
        /// which should be used for the specified file. If there is not appropriate decoder
        /// found, the method uses default .NET's image decoding routine (see
        /// <see cref="System.Drawing.Image.FromFile( string )"/>).</para></remarks>
        /// 
        public static Bitmap DecodeFromFile(string fileName, out ImageInfo imageInfo)
        {
            Bitmap bitmap = null;

            string fileExtension = Path.GetExtension(fileName).ToUpperInvariant();

            if ((fileExtension != string.Empty) && (fileExtension.Length != 0))
            {
                fileExtension = fileExtension.Substring(1);

                if (decoders.ContainsKey(fileExtension))
                {
                    IImageDecoder decoder = decoders[fileExtension];

                    // open stream
                    using (FileStream stream = new FileStream(fileName, FileMode.Open))
                    {
                        // open decoder
                        decoder.Open(stream);

                        // read the first frame
                        bitmap = decoder.DecodeFrame(0, out imageInfo);

                        decoder.Close();
                    }

                    return bitmap;
                }
            }

            // use default .NET's image decoding routine
            bitmap = FromFile(fileName);

            imageInfo = new ImageInfo(bitmap.Width, bitmap.Height, Image.GetPixelFormatSize(bitmap.PixelFormat), 0, 1);

            return bitmap;
        }

        private static System.Drawing.Bitmap FromFile(string fileName)
        {
            Bitmap loadedImage = null;

            // read image to temporary memory stream
            using (FileStream stream = File.OpenRead(fileName))
            {
                MemoryStream memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                loadedImage = (Bitmap)Bitmap.FromStream(memoryStream);
            }

            return loadedImage;
        }
    }
}
