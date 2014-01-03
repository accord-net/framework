// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Rémy Dispagne, 2013
// cramer at libertysurf.fr
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

namespace Accord.Math.Kinematics
{
    using System;

    /// <summary>
    ///    Denavit Hartenberg joint-description parameters.
    /// </summary>
    /// 
    [Serializable]
    public class DenavitHartenbergParameters
    {
        /// <summary>
        ///    Angle in radians about common normal, from 
        ///    old <c>z</c> axis to the new <c>z</c> axis.
        /// </summary>
        /// 
        public double Alpha { get; set; }

        /// <summary>
        ///    Angle in radians about previous <c>z</c>, 
        ///    from old <c>x</c> to the new <c>x</c>.
        /// </summary>
        /// 
        public double Theta { get; set; }

        /// <summary>
        ///    Length of the joint (also known as <c>a</c>).
        /// </summary>
        /// 
        public double Radius { get; set; }

        /// <summary>
        ///    Offset along previous <c>z</c> to the common normal (also known as <c>d</c>).
        /// </summary>
        /// 
        public double Offset { get; set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="DenavitHartenbergParameters"/> class.
        /// </summary>
        /// 
        /// <param name="alpha">Angle (in radians) of the Z axis relative to the last joint.</param>
        /// <param name="theta">Angle (in radians) of the X axis relative to the last joint.</param>
        /// <param name="radius">Length or radius of the joint.</param>
        /// <param name="offset">Offset along Z axis relatively to the last joint.</param>
        /// 
        public DenavitHartenbergParameters(double alpha, double theta, double radius, double offset)
        {
            this.Alpha = alpha;
            this.Theta = theta;
            this.Radius = (float)radius;
            this.Offset = (float)offset;
        }

        /// <summary>
        ///    Denavit Hartenberg parameters constructor
        /// </summary>
        /// 
        public DenavitHartenbergParameters()
        {
        }

    }
}
