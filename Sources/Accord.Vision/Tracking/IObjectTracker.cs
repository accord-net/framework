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

namespace Accord.Vision.Tracking
{
    using AForge.Imaging;
    using System.Collections.Generic;

    /// <summary>
    ///   Object tracker interface.
    /// </summary>
    /// 
    public interface IObjectTracker
    {
        /// <summary>
        ///   Gets the current location of the object being tracked.
        /// </summary>
        /// 
        TrackingObject TrackingObject { get; }

        /// <summary>
        ///   Process a new video frame.
        /// </summary>
        /// 
        void ProcessFrame(UnmanagedImage frame);

        /// <summary>
        ///   Gets or sets a value indicating whether the tracker should
        ///   extract the object image from the source. The extracted image
        ///   should be stored in <see cref="TrackingObject"/>.
        /// </summary>
        /// 
        bool Extract { get; set; }

    }
}
