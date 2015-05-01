// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace AForge.Math.Geometry
{
    using System;
    using AForge;
    using AForge.Math;

    /// <summary>
    /// 3D pose estimation algorithm.
    /// </summary>
    /// 
    /// <remarks><para>The class implements an algorithm for 3D object's pose estimation from it's
    /// 2D coordinates obtained by perspective projection, when the object is described none coplanar points.
    /// The idea of the implemented math and algorithm is described in "Model-Based Object Pose in 25
    /// Lines of Code" paper written by Daniel F. DeMenthon and Larry S. Davis (the implementation of
    /// the algorithm is almost 1 to 1 translation of the pseudo code given by the paper, so should
    /// be easy to follow).</para>
    /// 
    /// <para><note>At this point the implementation works only with models described by 4 points, which is
    /// the minimum number of points enough for 3D pose estimation.</note></para>
    /// 
    /// <para><note>The 4 model's point <b>must not be coplanar</b>, i.e. must not reside all within
    /// same planer. See <see cref="CoplanarPosit"/> for coplanar case.</note></para>
    /// 
    /// <para>Read <a href="http://www.aforgenet.com/articles/posit/">3D Pose Estimation</a> article for
    /// additional information and samples.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // points of real object - model
    /// Vector3[] positObject = new Vector3[4]
    /// { 
    ///     new Vector3(  28,  28, -28 ),
    ///     new Vector3( -28,  28, -28 ),
    ///     new Vector3(  28, -28, -28 ),
    ///     new Vector3(  28,  28,  28 ),
    /// };
    /// // focal length of camera used to capture the object
    /// float focalLength = 640; // depends on your camera or projection system
    /// // initialize POSIT object
    /// Posit posit = new Posit( positObject, focalLength );
    /// 
    /// // 2D points of te object - projection
    /// AForge.Point[] projectedPoints = new AForge.Point[4]
    /// {
    ///     new AForge.Point(   -4,   29 ),
    ///     new AForge.Point( -180,   86 ),
    ///     new AForge.Point(   -5, -102 ),
    ///     new AForge.Point(   76,  137 ),
    /// };
    /// // estimate pose
    /// Matrix3x3 rotationMatrix;
    /// Vector3 translationVector;
    /// posit.EstimatePose( projectedPoints,
    ///     out rotationMatrix, out translationVector );
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="CoplanarPosit"/>
    /// 
    public class Posit
    {
        // camera's focal length
        private float focalLength;

        // points of the model to estimate position for
        private Vector3[] modelPoints;
        // 3 vectors of the model kept as a matrix
        private Matrix3x3 modelVectors;
        // pseudoinverse of the model vectors matrix
        private Matrix3x3 modelPseudoInverse;

        /// <summary>
        /// Coordinates of the model points which pose should be estimated.
        /// </summary>
        public Vector3[] Model
        {
            get { return (Vector3[]) modelPoints.Clone( ); }
        }

        /// <summary>
        /// Effective focal length of the camera used to capture the model.
        /// </summary>
        public float FocalLength
        {
            get { return focalLength; }
            set { focalLength = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Posit"/> class.
        /// </summary>
        /// 
        /// <param name="model">Array of vectors containing coordinates of four real model's point (points
        /// must not be on the same plane).</param>
        /// <param name="focalLength">Effective focal length of the camera used to capture the model.</param>
        /// 
        /// <exception cref="ArgumentException">The model must have 4 points.</exception>
        /// 
        public Posit( Vector3[] model, float focalLength )
        {
            if ( model.Length != 4 )
            {
                throw new ArgumentException( "The model must have 4 points." );
            }

            this.focalLength = focalLength;
            modelPoints = (Vector3[]) model.Clone( );

            // compute model vectors
            modelVectors = Matrix3x3.CreateFromRows(
                model[1] - model[0],
                model[2] - model[0],
                model[3] - model[0] );

            // compute pseudo inverse matrix
            modelPseudoInverse = modelVectors.PseudoInverse( );
        }

        private const float stop_epsilon = 1.0e-4f;

        /// <summary>
        /// Estimate pose of a model from it's projected 2D coordinates.
        /// </summary>
        /// 
        /// <param name="points">4 2D points of the <see cref="Model">model's</see> projection.</param>
        /// <param name="rotation">Gets object's rotation.</param>
        /// <param name="translation">Gets object's translation.</param>
        /// 
        /// <exception cref="ArgumentException">4 points must be be given for pose estimation.</exception>
        /// 
        public void EstimatePose( Point[] points, out Matrix3x3 rotation, out Vector3 translation )
        {
            if ( points.Length != 4 )
            {
                throw new ArgumentException( "4 points must be be given for pose estimation." );
            }

            float Z0 = 0, scale = 1;

            Vector3 X0 = new Vector3( points[0].X );
            Vector3 Y0 = new Vector3( points[0].Y );

            Vector3 XI = new Vector3( points[1].X, points[2].X, points[3].X );
            Vector3 YI = new Vector3( points[1].Y, points[2].Y, points[3].Y );

            int count = 0;

            Vector3 iVector = new Vector3( );
            Vector3 jVector = new Vector3( );
            Vector3 kVector = new Vector3( );
            Vector3 imageXs = new Vector3( );
            Vector3 imageYs = new Vector3( );

            Vector3 eps = new Vector3( 1 );

            for ( ; count < 100; count++ )
            {
                // calculate new scale orthographic projection (SOP)
                imageXs = XI * eps - X0;
                imageYs = YI * eps - Y0;

                // calculate I and J vectors
                iVector = modelPseudoInverse * imageXs;
                jVector = modelPseudoInverse * imageYs;
                // convert them to unit vectors i and j
                float iNorm = iVector.Normalize( );
                float jNorm = jVector.Normalize( );
                // scale of projection
                scale = ( iNorm + jNorm ) / 2;
                // calculate n vector k
                kVector = Vector3.Cross( iVector, jVector );
                // z-coordinate Z0 of the translation vector
                Z0 = focalLength / scale;

                // calculate new epsilon values
                Vector3 oldEps = eps;
                eps = ( modelVectors * kVector ) / Z0 + 1;

                // check if it is time to stop
                if ( ( eps - oldEps ).Abs( ).Max < stop_epsilon )
                    break;
            }

            // create rotation matrix
            rotation = Matrix3x3.CreateFromRows( iVector, jVector, kVector );

            // create translation vector
            Vector3 temp = rotation * modelPoints[0];
            translation = new Vector3(
                points[0].X / scale - temp.X,
                points[0].Y / scale - temp.Y,
                focalLength / scale );
        }
    }
}
