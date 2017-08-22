// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//
// Accord Imaging Library
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

namespace Accord.Imaging
{
    /// <summary>
    /// Interface for custom blobs' filters used for filtering blobs after
    /// blob counting.
    /// </summary>
    /// 
    /// <remarks><para>The interface should be implemented by classes, which perform
    /// custom blobs' filtering different from default filtering implemented in
    /// <see cref="BlobCounterBase"/>. See <see cref="BlobCounterBase.BlobsFilter"/>
    /// for additional information.</para>
    /// </remarks>
    ///
    public interface IBlobsFilter
    {
        /// <summary>
        /// Check specified blob and decide if should be kept or not.
        /// </summary>
        /// 
        /// <param name="blob">Blob to check.</param>
        /// 
        /// <returns>Return <see langword="true"/> if the blob should be kept or
        /// <see langword="false"/> if it should be removed.</returns>
        ///
        bool Check(Blob blob);
    }
}
