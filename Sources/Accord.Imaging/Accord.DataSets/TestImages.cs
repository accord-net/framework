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
    using Accord;
    using Accord.Imaging;
    using Accord.Math;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
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
    /// <para>Using this class, you can retrieve any of the following famous test images:</para>
    /// 
    /// <list type="bullet">
    ///  <list><description>airplane.png  </description></list>
    ///  <list><description>arctichare.png</description></list>
    ///  <list><description>baboon.png    </description></list>
    ///  <list><description>barbara.bmp   </description></list>
    ///  <list><description>barbara.png   </description></list>
    ///  <list><description>boat.png      </description></list>
    ///  <list><description>boy.bmp       </description></list>
    ///  <list><description>boy.ppm       </description></list>
    ///  <list><description>cameraman.tif </description></list>
    ///  <list><description>cat.png       </description></list>
    ///  <list><description>fprint3.pgm   </description></list>
    ///  <list><description>fruits.png    </description></list>
    ///  <list><description>frymire.png   </description></list>
    ///  <list><description>girl.png      </description></list>
    ///  <list><description>goldhill.bmp  </description></list>
    ///  <list><description>goldhill.png  </description></list>
    ///  <list><description>lena.bmp      </description></list>
    ///  <list><description>lenacolor.png </description></list>
    ///  <list><description>lena.ppm      </description></list>
    ///  <list><description>Lenaclor.ppm  </description></list>
    ///  <list><description>monarch.png   </description></list>
    ///  <list><description>mountain.png  </description></list>
    ///  <list><description>mountain.bmp  </description></list>
    ///  <list><description>p64int.txt    </description></list>
    ///  <list><description>peppers.png   </description></list>
    ///  <list><description>pool.png      </description></list>
    ///  <list><description>sails.bmp     </description></list>
    ///  <list><description>sails.png     </description></list>
    ///  <list><description>serrano.png   </description></list>
    ///  <list><description>tulips.png    </description></list>
    ///  <list><description>us021.pgm     </description></list>
    ///  <list><description>us092.pgm     </description></list>
    ///  <list><description>watch.png     </description></list>
    ///  <list><description>zelda.png     </description></list>
    /// </list>
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
    /// <example>
    /// <code source="Unit Tests\Accord.Tests.Imaging\HistogramsOfOrientedGradientsTest.cs" region="doc_apply" />
    /// </example>
    /// 
    public class TestImages
    {
        string path;

        static readonly string[] opencv = new[]
        {
            "sudoku.png",
            "HappyFish.jpg",
            "lena.jpg",
            "text_defocus.jpg",
            "text_motion.jpg",
            "tree.avi"
        };

        static readonly string[] imageNames = new []
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
        }.Concatenate(opencv);


        /// <summary>
        ///   Gets all the image names that can be passed to
        ///   the <see cref="GetImage(string)"/> method.
        /// </summary>
        /// 
        /// <value>The image names in this dataset.</value>
        /// 
#if NET35 || NET40
        public string[] ImageNames
        {
            get { return (string[])imageNames.Clone(); }
        }
#else
        public IReadOnlyList<string> ImageNames
        {
            get { return imageNames; }
        }
#endif

        /// <summary>
        ///   Gets or sets whether images with non-standard color palettes (i.e. 8-bpp images where
        ///   values do not represent intensity values but rather indices in a color palette) should
        ///   be converted to true 8-bpp grayscale. Default is true.
        /// </summary>
        /// 
        /// <seealso cref="Accord.Imaging.Image.ConvertColor8bppToGrayscale8bpp"/>
        /// 
        public bool CorrectIndexedPalettes { get; set; }

        /// <summary>
        ///   Downloads and prepares the test images dataset.
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

            this.CorrectIndexedPalettes = true;
        }

        /// <summary>
        ///   Gets the example <see cref="Bitmap"/> with the specified name.
        /// </summary>
        /// 
        /// <param name="name">The standard image name. For a list of all possible names, see <see cref="ImageNames"/>.</param>
        /// 
        public Bitmap this[string name]
        {
            get { return GetImage(name); }
        }

        /// <summary>
        ///   Gets the example image.
        /// </summary>
        /// 
        /// <param name="name">The standard image name. For a list of all possible names, see <see cref="ImageNames"/>.</param>
        /// 
        public Bitmap GetImage(string name)
        {
            if (!imageNames.Contains(name))
            {
                throw new ArgumentOutOfRangeException("name", String.Format("The provided image '{0}' is not in the list of " +
                    "test images provided by this class. The list of supported image names can be found in the ImageNames " +
                    "property and in the Accord.DataSets.TestImages class documentation page.", name));
            }

            Bitmap bmp;

            if (opencv.Contains(name))
            {
                bmp = Accord.Imaging.Image.FromUrl("https://raw.githubusercontent.com/opencv/opencv/master/samples/data/" + name, path);
            }
            else
            {
                bmp = Accord.Imaging.Image.FromUrl("https://homepages.cae.wisc.edu/~ece533/images/" + name, path);
            }
            

            if (CorrectIndexedPalettes && bmp.IsColor8bpp())
                Accord.Imaging.Image.ConvertColor8bppToGrayscale8bpp(bmp);

            return bmp;
        }

    }
}
