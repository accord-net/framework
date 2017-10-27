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
    ///   Public-Domain test videos for video processing and vision applications.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This dataset contains sample videos from https://pixabay.com available under a CC0 Creative Commons
    ///   (a.k.a. public domain), meaning no they are free for commercial use and no attribution is required.</para>
    ///   
    /// <para>Using this class, you can retrieve any of the following test videos:</para>
    /// 
    /// <list type="bullet">
    ///  <list><description>walking.mp4</description></list>
    ///  <list><description>crowd.mp4</description></list>
    ///  <list><description>squirrel.mp4</description></list>
    /// </list>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://pixabay.com">
    ///       Pixabay. "Free Videos.", Hans Braxmeier and Simon Steinberger GbR, 2017.</a>
    ///       </description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code source="Sources\Extras\Accord.Tests.Video.FFMPEG\MatchingTrackerTest.cs" region="doc_track" />
    /// </example>
    /// 
    /// <seealso cref="TestImages"/>
    /// 
    public class TestVideos
    {
        string path;

        static readonly Dictionary<string, string> videoNames = new Dictionary<string, string>
        {
            { "crowd.mp4",    "https://onedrive.live.com/download?cid=808347681CC09388&resid=808347681CC09388%21530452&authkey=AAlYEvtLYUexCZk"},
            { "squirrel.mp4", "https://onedrive.live.com/download?cid=808347681CC09388&resid=808347681CC09388%21530451&authkey=AFsjxkumLlkYLWQ"},
            { "walking.mp4",  "https://onedrive.live.com/download?cid=808347681CC09388&resid=808347681CC09388%21530449&authkey=AA0k-21naqTD3wg"},
        };

        /// <summary>
        ///   Gets all the video names that can be passed to
        ///   the <see cref="GetVideo(string)"/> method.
        /// </summary>
        /// 
        /// <value>The video names in this dataset.</value>
        /// 
#if NET35 || NET40 || NET45 || MONO
        public string[] VideoNames
        {
            get { return videoNames.Keys.ToArray(); }
        }
#else
        public IReadOnlyCollection<string> VideoNames
        {
            get { return videoNames.Keys; }
        }
#endif

        /// <summary>
        ///   Downloads and prepares the test videos dataset.
        /// </summary>
        /// 
        /// <param name="path">The path where datasets will be stored. If null or empty, the dataset
        /// will be saved on a subfolder called "data" in the current working directory.</param>
        /// 
        public TestVideos(string path = null)
        {
            if (path == null)
                path = "data";
            this.path = path;
        }

        /// <summary>
        ///   Downloads the example video with the specified name and returns the file path where it has been saved.
        /// </summary>
        /// 
        /// <param name="name">The video name. For a list of all possible names, see <see cref="VideoNames"/>.</param>
        /// 
        public string this[string name]
        {
            get { return GetVideo(name); }
        }

        /// <summary>
        ///   Downloads the example video with the specified name and returns the file path where it has been saved.
        /// </summary>
        /// 
        /// <param name="name">The standard video name. For a list of all possible names, see <see cref="VideoNames"/>.</param>
        /// 
        public string GetVideo(string name)
        {
            if (!videoNames.ContainsKey(name))
            {
                throw new ArgumentOutOfRangeException("name", String.Format("The provided video '{0}' is not in the list of " +
                    "test videos provided by this class. The list of supported video names can be found in the VideoNames " +
                    "property and in the Accord.DataSets.TestVideos class documentation page.", name));
            }

            string url = videoNames[name];

            string downloadedFileName = System.IO.Path.Combine(this.path, name);

            if (!File.Exists(downloadedFileName))
            {
#if NET35
                if (this.path == null || String.IsNullOrEmpty(this.path.Trim()))
#else
                if (!String.IsNullOrWhiteSpace(this.path))
#endif
                    Directory.CreateDirectory(this.path);

                using (var client = ExtensionMethods.NewWebClient())
                {
                    Console.WriteLine("Downloading {0}", url);
                    client.DownloadFileWithRetry(url, downloadedFileName);
                }
            }

            return downloadedFileName;
        }

    }
}
