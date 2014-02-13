// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Pablo Guzman Sanchez, 2013
// pablogsanchez at gmail.com
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
// This code originated as a contribution by Pablo Sanches, originally based on
// Student Dave's tutorial on Object Tracking in Images Using 2D Kalman Filters:
//
//   http://studentdavestutorials.weebly.com/object-tracking-2d-kalman-filter.html
//

namespace Accord.Statistics.Running
{
    using Accord.Math;
    using AForge;

    /// <summary>
    ///   Kalman filter for 2D coordinate systems.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://studentdavestutorials.weebly.com/object-tracking-2d-kalman-filter.html">
    ///       Student Dave's tutorial on Object Tracking in Images Using 2D Kalman Filters.
    ///       Available on: http://studentdavestutorials.weebly.com/object-tracking-2d-kalman-filter.html
    ///       </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    public class KalmanFilter2D : IRunning<DoublePoint>
    {

        double samplingRate = 1;

        double acceleration = 0.005f;
        double accelStdDev = 0.1f;

        double[,] Q_estimate;

        double[,] A;
        double[,] B;
        double[,] C;

        double[,] Ez;
        double[,] Ex;
        double[,] P;
        double[,] K;
        double[,] Aux;

        static readonly double[,] diagonal = 
        {
            { 1, 0, 0, 0 },
            { 0, 1, 0, 0 },
            { 0, 0, 1, 0 },
            { 0, 0, 0, 1 }
        };

        /// <summary>
        ///   Gets or sets the current X position of the object.
        /// </summary>
        /// 
        public double X
        {
            get { return Q_estimate[0, 0]; }
            set { Q_estimate[0, 0] = value; }
        }

        /// <summary>
        ///   Gets or sets the current Y position of the object.
        /// </summary>
        /// 
        public double Y
        {
            get { return Q_estimate[1, 0]; }
            set { Q_estimate[1, 0] = value; }
        }

        /// <summary>
        ///   Gets or sets the current object's velocity in the X axis.
        /// </summary>
        /// 
        public double XAxisVelocity
        {
            get { return Q_estimate[2, 0]; }
            set { Q_estimate[2, 0] = value; }
        }

        /// <summary>
        ///   Gets or sets the current object's velocity in the Y axis.
        /// </summary>
        /// 
        public double YAxisVelocity
        {
            get { return Q_estimate[3, 0]; }
            set { Q_estimate[3, 0] = value; }
        }

        /// <summary>
        ///   Gets or sets the observational noise 
        ///   of the current object's in the X axis.
        /// </summary>
        /// 
        public double NoiseX
        {
            get { return Ez[0, 0]; }
            set { Ez[0, 0] = value; }
        }

        /// <summary>
        ///   Gets or sets the observational noise 
        ///   of the current object's in the Y axis.
        /// </summary>
        /// 
        public double NoiseY
        {
            get { return Ez[1, 1]; }
            set { Ez[1, 1] = value; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="KalmanFilter2D"/> class.
        /// </summary>
        /// 
        public KalmanFilter2D()
        {
            initialize();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="KalmanFilter2D"/> class.
        /// </summary>
        /// 
        /// <param name="samplingRate">The sampling rate.</param>
        /// <param name="acceleration">The acceleration.</param>
        /// <param name="accelerationStdDev">The acceleration standard deviation.</param>
        /// 
        public KalmanFilter2D(double samplingRate,
            double acceleration, double accelerationStdDev)
        {
            this.acceleration = acceleration;
            this.accelStdDev = accelerationStdDev;
            this.samplingRate = samplingRate;

            initialize();
        }

        private void initialize()
        {
            double dt = samplingRate;

            A = new double[,] 
            {
                { 1,  0, dt,  0 },
                { 0,  1,  0, dt },
                { 0,  0,  1,  0 },
                { 0,  0,  0,  1 }
            };

            B = new double[,] 
            {
                { (dt * dt) / 2 },
                { (dt * dt) / 2 },
                {       dt      }, 
                {       dt      }
            };

            C = new double[,]
            {
                { 1, 0, 0, 0 },
                { 0, 1, 0, 0 }
            };

            Ez = new double[2, 2];

            double dt2 = dt * dt;
            double dt3 = dt2 * dt;
            double dt4 = dt2 * dt2;

            double aVar = accelStdDev * accelStdDev;

            Ex = new double[4, 4]
            {
                { dt4 / 4,        0,  dt3 / 2,        0 },
                { 0,        dt4 / 4,        0,  dt3 / 2 },
                { dt3 / 2,        0,      dt2,        0 },
                { 0,        dt3 / 2,        0,      dt2 }
            }.Multiply(aVar, inPlace: true);


            P = Ex.MemberwiseClone();
        }


        /// <summary>
        ///   Registers the occurrence of a value.
        /// </summary>
        /// 
        /// <param name="value">The value to be registered.</param>
        /// 
        public void Push(DoublePoint value)
        {
            double[,] Qloc = { { value.X }, { value.Y } };

            // Predict next state
            Q_estimate = (A.Multiply(Q_estimate)).Add(B.Multiply(acceleration));

            // Predict Covariances
            P = (A.Multiply(P.Multiply(A.Transpose()))).Add(Ex);

            Aux = (C.Multiply(P.Multiply(C.Transpose())).Add(Ez)).PseudoInverse();

            // Kalman Gain
            K = P.Multiply(C.Transpose().Multiply(Aux));
            Q_estimate = Q_estimate.Add(K.Multiply(Qloc.Subtract(C.Multiply(Q_estimate))));

            // Update P (Covariances)
            P = (diagonal.Subtract(K.Multiply(C))).Multiply(P);
        }


        /// <summary>
        ///   Clears all measures previously computed.
        /// </summary>
        /// 
        public void Clear()
        {
            this.NoiseX = 0;
            this.NoiseY = 0;

            this.XAxisVelocity = 0;
            this.YAxisVelocity = 0;

            this.X = 0;
            this.Y = 0;
        }
    }
}
