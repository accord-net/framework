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
    using AForge.Math;

    /// <summary>
    ///   Denavit Hartenberg matrix (commonly referred as T).
    /// </summary>
    /// 
    public class DenavitHartenbergMatrix
    {
        /// <summary>
        ///   Gets or sets the transformation matrix T (as in T = Z * X).
        /// </summary>
        /// 
        public Matrix4x4 Transform { get; set; }

        /// <summary>
        ///   Gets or sets the matrix regarding X axis transformations.
        /// </summary>
        /// 
        public Matrix4x4 X { get; set; }
        
        /// <summary>
        ///   Gets or sets the matrix regarding Z axis transformations.
        /// </summary>
        /// 
        public Matrix4x4 Z { get; set; }

    

        /// <summary>
        ///   Executes the transform calculations (T = Z*X).
        /// </summary>
        /// 
        /// <returns>Transform matrix T.</returns>
        /// 
        /// <remarks>Calling this method also updates the Transform property.</remarks>
        /// 
        public void Compute(DenavitHartenbergParameters parameters)
        {
            // Calculate Z with Z = TranslationZ(d).RotationZ(theta)
            Z = Matrix4x4.Multiply
            (
                Matrix4x4.CreateTranslation(new Vector3(0f, 0f, (float)parameters.Offset)),
                Matrix4x4.CreateRotationZ((float)parameters.Theta)
            );

            // Calculate X with X = TranslationX(radius).RotationZ(alpha)
            X = Matrix4x4.Multiply
            (
                Matrix4x4.CreateTranslation(new Vector3((float)parameters.Radius, 0f, 0f)),
                Matrix4x4.CreateRotationX((float)parameters.Alpha)
            );

            // Calculate the transform with T=Z.X
            Transform = Matrix4x4.Multiply(Z, X);
        }
    }
}
