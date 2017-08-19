// Accord Video Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// AForge Video Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2012
// contacts@aforgenet.com
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

namespace Accord.Video
{
    using System;

    /// <summary>
    /// Video related exception.
    /// </summary>
    /// 
    /// <remarks><para>The exception is thrown in the case of some video related issues, like
    /// failure of initializing codec, compression, etc.</para></remarks>
    /// 
    public class VideoException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VideoException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Exception's message.</param>
        /// 
        public VideoException( string message ) :
            base( message ) { }
    }
}
