// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging
{
    using System;
    using System.Drawing;

    /// <summary>
    /// Block match class keeps information about found block match. The class is
    /// used with block matching algorithms implementing <see cref="IBlockMatching"/>
    /// interface.
    /// </summary>
    /// 
    public class BlockMatch
    {
        private IntPoint sourcePoint;
        private IntPoint matchPoint;
        private float similarity;

        /// <summary>
        /// Reference point in source image.
        /// </summary>
        public IntPoint SourcePoint
        {
            get { return sourcePoint; }
        }

        /// <summary>
        /// Match point in search image (point of a found match).
        /// </summary>
        public IntPoint MatchPoint
        {
            get { return matchPoint; }
        }

        /// <summary>
        /// Similarity between blocks in source and search images, [0..1].
        /// </summary>
        public float Similarity
        {
            get { return similarity; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockMatch"/> class.
        /// </summary>
        /// 
        /// <param name="sourcePoint">Reference point in source image.</param>
        /// <param name="matchPoint">Match point in search image (point of a found match).</param>
        /// <param name="similarity">Similarity between blocks in source and search images, [0..1].</param>
        /// 
        public BlockMatch( IntPoint sourcePoint, IntPoint matchPoint, float similarity )
        {
            this.sourcePoint = sourcePoint;
            this.matchPoint  = matchPoint;
            this.similarity  = similarity;
        }
    }
}
