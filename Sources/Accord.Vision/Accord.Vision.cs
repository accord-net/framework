// Accord Vision Library
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

namespace Accord.Vision
{
    using System.Runtime.CompilerServices;
    using Accord.Vision.Detection;
    using Accord.Vision.Detection.Cascades;
    using Accord.Vision.Tracking;

    /// <summary>
    ///   Real-time face detection and tracking, as well as general methods for detecting,
    ///   tracking and transforming objects in image streams. Contains cascade definitions,
    ///   Camshift and Dynamic Template Matching trackers.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The <c>Accord.Vision</c> namespace houses the popular <see cref="HaarObjectDetector">
    ///   Haar Cascade Classifier</see>, also known as the Viola-Jones object detector. This
    ///   namespace also offers diverse <see cref="Accord.Vision.Detection.Cascades">cascade 
    ///   definitions</see> which can be used to track different parts of the human body. Custom
    ///   cascades <see cref="HaarCascade.FromXml(string)">can also be loaded from OpenCV-compatible
    ///   XML cascade definition files</see>, and should work seamlessly even when containing
    ///   tilted cascade features, or multi-node (tree nodes) cascade definitions.</para>
    ///   
    /// <para>
    ///   The namespace also offers tracking algorithms, such as Bradski's <see cref="Camshift"/>,
    ///   the much simpler <see cref="HslBlobTracker">HSL blob tracker algorithm</see> and even a
    ///   <see cref="MatchingTracker">template matching tracker</see>.
    /// </para>
    /// 
    /// <para>
    ///   The namespace class diagram is shown below. </para>
    ///   <img src="..\diagrams\classes\Accord.Vision.png" />
    ///   
    /// </remarks>
    /// 
    /// <seealso cref="Accord.Vision.Detection"/>
    /// <seealso cref="Accord.Vision.Detection.Cascades"/>
    /// <seealso cref="Accord.Vision.Tracking"/>
    /// 
    [CompilerGenerated]
    class NamespaceDoc
    {
    }
}
