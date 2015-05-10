// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace AForge.Imaging.Filters
{
	/// <summary>
	/// Blur filter.
	/// </summary>
	/// 
    /// <remarks><para>The filter performs <see cref="Convolution">convolution filter</see> using
    /// the blur kernel:</para>
    /// 
    /// <code lang="none">
    /// 1  2  3  2  1
    /// 2  4  5  4  2
    /// 3  5  6  5  3
    /// 2  4  5  4  2
    /// 1  2  3  2  1
    /// </code>
    /// 
    /// <para>For the list of supported pixel formats, see the documentation to <see cref="Convolution"/>
    /// filter.</para>
    /// 
    /// <para><note>By default this filter sets <see cref="Convolution.ProcessAlpha"/> property to
    /// <see langword="true"/>, so the alpha channel of 32 bpp and 64 bpp images is blurred as well.
    /// </note></para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// Blur filter = new Blur( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    ///
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/blur.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    /// <seealso cref="Convolution"/>
    /// 
    public sealed class Blur : Convolution
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Blur"/> class.
		/// </summary>
		public Blur( ) : base( new int[,] {
								{ 1, 2, 3, 2, 1 },
								{ 2, 4, 5, 4, 2 },
								{ 3, 5, 6, 5, 3 },
								{ 2, 4, 5, 4, 2 },
								{ 1, 2, 3, 2, 1 } } )
		{
            base.ProcessAlpha = true;
		}
	}
}
