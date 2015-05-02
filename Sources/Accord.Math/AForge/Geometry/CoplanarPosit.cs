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
    /// 3D pose estimation algorithm (coplanar case).
    /// </summary>
    ///
    /// <remarks><para>The class implements an algorithm for 3D object's pose estimation from it's
    /// 2D coordinates obtained by perspective projection, when the object is described coplanar points.
    /// The idea of the implemented math and algorithm is described in "Iterative Pose Estimation using
    /// Coplanar Feature Points" paper written by Oberkampf, Daniel DeMenthon and Larry Davis
    /// (the implementation of the algorithm is very close translation of the pseudo code given by the
    /// paper, so should be easy to follow).</para>
    /// 
    /// <para><note>At this point the implementation works only with models described by 4 points, which is
    /// the minimum number of points enough for 3D pose estimation.</note></para>
    /// 
    /// <para><note>The 4 model's point are supposed to be coplanar, i.e. supposed to reside all within
    /// same planer. See <see cref="Posit"/> for none coplanar case.</note></para>
    /// 
    /// <para>Read <a href="http://www.aforgenet.com/articles/posit/">3D Pose Estimation</a> article for
    /// additional information and samples.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // points of real object - model
    /// Vector3[] copositObject = new Vector3[4]
    /// { 
    ///     new Vector3( -56.5f, 0,  56.5f ),
    ///     new Vector3(  56.5f, 0,  56.5f ),
    ///     new Vector3(  56.5f, 0, -56.5f ),
    ///     new Vector3( -56.5f, 0, -56.5f ),
    /// };
    /// // focal length of camera used to capture the object
    /// float focalLength = 640; // depends on your camera or projection system
    /// // initialize CoPOSIT object
    /// CoplanarPosit coposit = new CoplanarPosit( copositObject, focalLength );
    /// 
    /// // 2D points of te object - projection
    /// AForge.Point[] projectedPoints = new AForge.Point[4]
    /// {
    ///     new AForge.Point( -77,  48 ),
    ///     new AForge.Point(  44,  66 ),
    ///     new AForge.Point(  75, -36 ),
    ///     new AForge.Point( -61, -58 ),
    /// };
    /// // estimate pose
    /// Matrix3x3 rotationMatrix;
    /// Vector3 translationVector;
    /// coposit.EstimatePose( projectedPoints,
    ///     out rotationMatrix, out translationVector );
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="Posit"/>
    ///
    public class CoplanarPosit
    {
        // camera's focal length
        private float focalLength;

        // points of the model to estimate position for
        private Vector3[] modelPoints;
        // 3 vectors of the model kept as a matrix
        private Matrix3x3 modelVectors;
        // pseudoinverse of the model vectors matrix
        private Matrix3x3 modelPseudoInverse;
        // unit normal to the model
        private Vector3 modelNormal;

        private Matrix3x3 alternateRotation = new Matrix3x3( );
        private Vector3   alternateTranslation = new Vector3( );
        private float     alternatePoseError = 0;

        private Matrix3x3 bestRotation = new Matrix3x3( );
        private Vector3   bestTranslation = new Vector3( );
        private float     bestPoseError = 0;

        /// <summary>
        /// Best estimated pose recently found.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps best estimated pose found by the latest call to <see cref="EstimatePose"/>.
        /// The same estimated pose is provided by that method also and can be accessed through this property
        /// for convenience.</para>
        /// 
        /// <para>See also <see cref="BestEstimatedTranslation"/> and <see cref="BestEstimationError"/>.</para>
        /// </remarks>
        /// 
        public Matrix3x3 BestEstimatedRotation
        {
            get { return bestRotation; }
        }

        /// <summary>
        /// Best estimated translation recently found.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps best estimated translation found by the latest call to <see cref="EstimatePose"/>.
        /// The same estimated translation is provided by that method also and can be accessed through this property
        /// for convenience.</para>
        /// 
        /// <para>See also <see cref="BestEstimatedRotation"/> and <see cref="BestEstimationError"/>.</para>
        /// </remarks>
        /// 
        public Vector3 BestEstimatedTranslation
        {
            get { return bestTranslation; }
        }

        /// <summary>
        /// Error of the best pose estimation.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps error of the best pose estimation, which is calculated as average
        /// error between real angles of the specified quadrilateral and angles of the quadrilateral which
        /// is a projection of the best pose estimation. The error is measured degrees in (angle).</para>
        /// </remarks>
        /// 
        public float BestEstimationError
        {
            get { return bestPoseError; }
        }

        /// <summary>
        /// Alternate estimated pose recently found.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps alternate estimated pose found by the latest call to <see cref="EstimatePose"/>.</para>
        /// 
        /// <para>See also <see cref="AlternateEstimatedTranslation"/> and <see cref="AlternateEstimationError"/>.</para>
        /// </remarks>
        /// 
        public Matrix3x3 AlternateEstimatedRotation
        {
            get { return alternateRotation; }
        }

        /// <summary>
        /// Alternated estimated translation recently found.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps alternate estimated translation found by the latest call to <see cref="EstimatePose"/>.</para>
        /// 
        /// <para>See also <see cref="AlternateEstimatedRotation"/> and <see cref="AlternateEstimationError"/>.</para>
        /// </remarks>
        /// 
        public Vector3 AlternateEstimatedTranslation
        {
            get { return alternateTranslation; }
        }

        /// <summary>
        /// Error of the alternate pose estimation.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps error of the alternate pose estimation, which is calculated as average
        /// error between real angles of the specified quadrilateral and angles of the quadrilateral which
        /// is a projection of the alternate pose estimation. The error is measured in degrees (angle).</para>
        /// </remarks>
        /// 
        public float AlternateEstimationError
        {
            get { return alternatePoseError; }
        }

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
        /// <param name="model">Array of vectors containing coordinates of four real model's point.</param>
        /// <param name="focalLength">Effective focal length of the camera used to capture the model.</param>
        /// 
        /// <exception cref="ArgumentException">The model must have 4 points.</exception>
        /// 
        public CoplanarPosit( Vector3[] model, float focalLength )
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

            // compute pseudo inverse of the model matrix
            Matrix3x3 u, v;
            Vector3 e;

            modelVectors.SVD( out u, out e, out v );
            modelPseudoInverse = v * Matrix3x3.CreateDiagonal( e.Inverse( ) ) * u.Transpose( );

            // computer unit vector normal to the model
            modelNormal = v.GetColumn( e.MinIndex );
        }

        /// <summary>
        /// Estimate pose of a model from it's projected 2D coordinates.
        /// </summary>
        /// 
        /// <param name="points">4 2D points of the <see cref="Model">model's</see> projection.</param>
        /// <param name="rotation">Gets best estimation of object's rotation.</param>
        /// <param name="translation">Gets best estimation of object's translation.</param>
        /// 
        /// <exception cref="ArgumentException">4 points must be be given for pose estimation.</exception>
        /// 
        /// <remarks><para>Because of the Coplanar POSIT algorithm's nature, it provides two pose estimations,
        /// which are valid from the algorithm's math point of view. For each pose an error is calculated,
        /// which specifies how good estimation fits to the specified real 2D coordinated. The method
        /// provides the best estimation through its output parameters <paramref name="rotation"/> and
        /// <paramref name="translation"/>. This may be enough for many of the pose estimation application.
        /// For those, who require checking the alternate pose estimation, it can be obtained using
        /// <see cref="AlternateEstimatedRotation"/> and <see cref="AlternateEstimatedTranslation"/> properties.
        /// The calculated error is provided for both estimations through <see cref="BestEstimationError"/> and
        /// <see cref="AlternateEstimationError"/> properties.</para>
        /// </remarks>
        /// 
        public void EstimatePose( Point[] points, out Matrix3x3 rotation, out Vector3 translation )
        {
            if ( points.Length != 4 )
            {
                throw new ArgumentException( "4 points must be be given for pose estimation." );
            }

            Matrix3x3 rotation1, rotation2;
            Vector3 translation1, translation2;

            // find initial rotation
            POS( points, new Vector3( 1 ), out rotation1, out rotation2, out translation1, out translation2 );

            // iterate further and fine tune the solution
            float error1 = Iterate( points, ref rotation1, ref translation1 );
            float error2 = Iterate( points, ref rotation2, ref translation2 );

            // take the best found pose
            if ( error1 < error2 )
            {
                bestRotation    = rotation1;
                bestTranslation = translation1;
                bestPoseError   = error1;

                alternateRotation    = rotation2;
                alternateTranslation = translation2;
                alternatePoseError   = error2;
            }
            else
            {
                bestRotation    = rotation2;
                bestTranslation = translation2;
                bestPoseError   = error2;

                alternateRotation    = rotation1;
                alternateTranslation = translation1;
                alternatePoseError   = error1;
            }

            rotation    = bestRotation;
            translation = bestTranslation;
        }

        private const float ErrorLimit = 2;

        // Iterate POS algorithm starting from the specified rotation and translation and fine tune it
        private float Iterate( Point[] points, ref Matrix3x3 rotation, ref Vector3 translation )
        {
            float prevError = float.MaxValue;
            float error = 0;

            // run maximum 100 iterations (seems to be overkill, since typicaly it requires around 1-2 iterations)
            for ( int count = 0; count < 100; count++ )
            {
                Matrix3x3 rotation1, rotation2;
                Vector3 translation1, translation2;

                // calculates new epsilon values
                Vector3 eps = ( modelVectors * rotation.GetRow( 2 ) ) / translation.Z + 1;
                // and new pose
                POS( points, eps, out rotation1, out rotation2, out translation1, out translation2 );

                // calculate error for both new poses
                float error1 = GetError( points, rotation1, translation1 );
                float error2 = GetError( points, rotation2, translation2 );

                // select the pose which gives smaller error
                if ( error1 < error2 )
                {
                    rotation    = rotation1;
                    translation = translation1;
                    error       = error1;
                }
                else
                {
                    rotation    = rotation2;
                    translation = translation2;
                    error       = error2;
                }

                // stop if error is small enough or started to grow
                if ( ( error <= ErrorLimit ) || ( error > prevError ) )
                    break;

                prevError = error;
            }

            return error;
        }

        // Perform single iteration of POS (pos estimations) algorithm to find possible rotations and translation vectors
        private void POS( Point[] imagePoints, Vector3 eps, out Matrix3x3 rotation1, out Matrix3x3 rotation2, out Vector3 translation1, out Vector3 translation2 )
        {
            // create vectors keeping all X and Y coordinates for the 1st, 2nd and 3rd points
            Vector3 XI = new Vector3( imagePoints[1].X, imagePoints[2].X, imagePoints[3].X );
            Vector3 YI = new Vector3( imagePoints[1].Y, imagePoints[2].Y, imagePoints[3].Y );

            // calculate scale orthographic projection (SOP)
            Vector3 imageXs = XI * eps - imagePoints[0].X;
            Vector3 imageYs = YI * eps - imagePoints[0].Y;

            // calculate I0 and J0 vectors
            Vector3 I0Vector = modelPseudoInverse * imageXs;
            Vector3 J0Vector = modelPseudoInverse * imageYs;

            Vector3 iVector = new Vector3( );
            Vector3 jVector = new Vector3( );
            Vector3 kVector = new Vector3( );

            // find roots of complex number C^2
            float j2i2dif = J0Vector.Square - I0Vector.Square;
            float ij = Vector3.Dot( I0Vector, J0Vector );

            float r = 0, theta = 0;

            if ( j2i2dif == 0 )
            {
                theta = (float) ( ( -System.Math.PI / 2 ) * System.Math.Sign( ij ) );
                r = (float) System.Math.Sqrt( System.Math.Abs( 2 * ij ) );
            }
            else
            {
                r = (float) System.Math.Sqrt( System.Math.Sqrt( j2i2dif * j2i2dif + 4 * ij * ij ) );
                theta = (float) System.Math.Atan( -2 * ij / j2i2dif );

                if ( j2i2dif < 0 )
                    theta += (float) System.Math.PI;

                theta /= 2;
            }

            float lambda = (float) ( r * System.Math.Cos( theta ) );
            float mu =     (float) ( r * System.Math.Sin( theta ) );

            // first possible rotation
            iVector = I0Vector + ( modelNormal * lambda );
            jVector = J0Vector + ( modelNormal * mu );

            float iNorm = iVector.Normalize( );
            float jNorm = jVector.Normalize( );
            kVector = Vector3.Cross( iVector, jVector );

            rotation1 = Matrix3x3.CreateFromRows( iVector, jVector, kVector );

            // calculate translation vector
            float scale = ( iNorm + jNorm ) / 2;

            Vector3 temp = rotation1 * modelPoints[0];
            translation1 = new Vector3( imagePoints[0].X / scale - temp.X, imagePoints[0].Y / scale - temp.Y, focalLength / scale );

            // second possible rotation
            iVector = I0Vector - ( modelNormal * lambda );
            jVector = J0Vector - ( modelNormal * mu );

            iNorm = iVector.Normalize( );
            jNorm = jVector.Normalize( );
            kVector = Vector3.Cross( iVector, jVector );

            rotation2 = Matrix3x3.CreateFromRows( iVector, jVector, kVector );

            scale = ( iNorm + jNorm ) / 2;

            temp = rotation2 * modelPoints[0];
            translation2 = new Vector3( imagePoints[0].X / scale - temp.X, imagePoints[0].Y / scale - temp.Y, focalLength / scale );
        }

        // Calculate average error between real angles of the specified quadrilateral and angles of the
        // quadrilateral which is the projection of currently estimated pose
        private float GetError( Point[] imagePoints, Matrix3x3 rotation, Vector3 translation )
        {
            Vector3 v1 = rotation * modelPoints[0] + translation;
            v1.X = v1.X * focalLength / v1.Z;
            v1.Y = v1.Y * focalLength / v1.Z;

            Vector3 v2 = rotation * modelPoints[1] + translation;
            v2.X = v2.X * focalLength / v2.Z;
            v2.Y = v2.Y * focalLength / v2.Z;

            Vector3 v3 = rotation * modelPoints[2] + translation;
            v3.X = v3.X * focalLength / v3.Z;
            v3.Y = v3.Y * focalLength / v3.Z;

            Vector3 v4 = rotation * modelPoints[3] + translation;
            v4.X = v4.X * focalLength / v4.Z;
            v4.Y = v4.Y * focalLength / v4.Z;

            Point[] modeledPoints = new Point[4]
            {
                new Point( v1.X, v1.Y ),
                new Point( v2.X, v2.Y ),
                new Point( v3.X, v3.Y ),
                new Point( v4.X, v4.Y ),
            };

            float ia1 = GeometryTools.GetAngleBetweenVectors( imagePoints[0], imagePoints[1], imagePoints[3] );
            float ia2 = GeometryTools.GetAngleBetweenVectors( imagePoints[1], imagePoints[2], imagePoints[0] );
            float ia3 = GeometryTools.GetAngleBetweenVectors( imagePoints[2], imagePoints[3], imagePoints[1] );
            float ia4 = GeometryTools.GetAngleBetweenVectors( imagePoints[3], imagePoints[0], imagePoints[2] );

            float ma1 = GeometryTools.GetAngleBetweenVectors( modeledPoints[0], modeledPoints[1], modeledPoints[3] );
            float ma2 = GeometryTools.GetAngleBetweenVectors( modeledPoints[1], modeledPoints[2], modeledPoints[0] );
            float ma3 = GeometryTools.GetAngleBetweenVectors( modeledPoints[2], modeledPoints[3], modeledPoints[1] );
            float ma4 = GeometryTools.GetAngleBetweenVectors( modeledPoints[3], modeledPoints[0], modeledPoints[2] );

            return (
                System.Math.Abs( ia1 - ma1 ) +
                System.Math.Abs( ia2 - ma2 ) +
                System.Math.Abs( ia3 - ma3 ) +
                System.Math.Abs( ia4 - ma4 )
                ) / 4;
        }

        // This function calculates error value as it is mentioned in theory - average distance
        // between image points and projected model's points. However, if translation is not
        // calculated very precisely (because of inaccurate focal length, for example), this function
        // may return big error value for both estimated poses. So the above function uses
        // angles between objects' side instead of corners' coordinates.
        /*
        private float _GetError( Point[] imagePoints, Matrix3x3 rotation, Vector3 translation )
        {
            Vector3 v1 = rotation * modelPoints[0] + translation;
            v1.X = v1.X * focalLength / v1.Z;
            v1.Y = v1.Y * focalLength / v1.Z;

            Vector3 v2 = rotation * modelPoints[1] + translation;
            v2.X = v2.X * focalLength / v2.Z;
            v2.Y = v2.Y * focalLength / v2.Z;

            Vector3 v3 = rotation * modelPoints[2] + translation;
            v3.X = v3.X * focalLength / v3.Z;
            v3.Y = v3.Y * focalLength / v3.Z;

            Vector3 v4 = rotation * modelPoints[3] + translation;
            v4.X = v4.X * focalLength / v4.Z;
            v4.Y = v4.Y * focalLength / v4.Z;

            Point[] modeledPoints = new Point[4]
            {
                new Point( v1.X, v1.Y ),
                new Point( v2.X, v2.Y ),
                new Point( v3.X, v3.Y ),
                new Point( v4.X, v4.Y ),
            };

            return (
                imagePoints[0].DistanceTo( modeledPoints[0] ) +
                imagePoints[1].DistanceTo( modeledPoints[1] ) +
                imagePoints[2].DistanceTo( modeledPoints[2] ) +
                imagePoints[3].DistanceTo( modeledPoints[3] )
                ) / 4;
        }
        */
    }
}
