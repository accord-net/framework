// Accord Datasets Library
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

namespace Accord.DataSets
{
    using Accord.Math;
    using System;
    using System.Drawing;
    using System.IO;
    using System.Net;

    /// <summary>
    ///   Public-Domain test images for image processing applications.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This dataset contains famous images used in the image processing literature, such as 
    ///   <a href="https://en.wikipedia.org/wiki/Lenna">Lena Söderberg</a> picture.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://homepages.cae.wisc.edu/~ece533/images/">
    ///       ECE533 Digital Image Processing. "Public-Domain Test Images for Homeworks and Projects.",
    ///       University of Wisconsin-Madison, Fall 2012.</a>
    ///       </description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    public class TestImages
    {

        string path;

        private string[] imageNames = new[]
        {
            "airplane.png",
            "arctichare.png",
            "baboon.png",
            "barbara.bmp",
            "barbara.png",
            "boat.png",
            "boy.bmp",
            "boy.ppm",
            "cameraman.tif",
            "cat.png",
            "fprint3.pgm",
            "fruits.png",
            "frymire.png",
            "girl.png",
            "goldhill.bmp",
            "goldhill.png",
            "lena.bmp",
            "lenacolor.png",
            "lena.ppm",
            "Lenaclor.ppm",
            "monarch.png",
            "mountain.png",
            "mountain.bmp",
            "p64int.txt",
            "peppers.png",
            "pool.png",
            "sails.bmp",
            "sails.png",
            "serrano.png",
            "tulips.png",
            "us021.pgm",
            "us092.pgm",
            "watch.png",
            "zelda.png",
        };

        /// <summary>
        ///   Gets all the image names that can be passed to
        ///   the <see cref="GetImage(string)"/> method.
        /// </summary>
        /// 
        /// <value>The image names in this dataset.</value>
        /// 
        public string[] ImageNames
        {
            get { return imageNames; }
        }

        /// <summary>
        ///   Downloads and prepares the Iris dataset.
        /// </summary>
        /// 
        /// <param name="path">The path where datasets will be stored. If null or empty, the dataset
        /// will be saved on a subfolder called "data" in the current working directory.</param>
        /// 
        public TestImages(string path = null)
        {
            if (path == null)
                path = "data";
            this.path = path;
        }

        /// <summary>
        ///   Gets the example image.
        /// </summary>
        /// 
        /// <param name="filename">The filename of the image to be retrieved.</param>
        /// 
        public Bitmap GetImage(string filename)
        {
            return Accord.Imaging.Image.FromUrl("https://homepages.cae.wisc.edu/~ece533/images/" + filename, path);
        }

    }
}
