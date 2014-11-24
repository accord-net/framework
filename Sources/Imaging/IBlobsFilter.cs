// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2010
// andrew.kirillov@aforgenet.com

namespace AForge.Imaging
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
        bool Check( Blob blob );
    }
}
