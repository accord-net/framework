// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
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

namespace Accord.Math.Environments
{
    using System.CodeDom.Compiler;
    using Accord.Math;

    /// <summary>
    ///   GNU R algorithm environment. Work in progress.
    /// </summary>
    /// 
    [GeneratedCode("", "")]
    public abstract class REnvironment
    {
        /// <summary>
        ///   Creates a new vector.
        /// </summary>
        /// 
        protected vec c(params double[] values)
        {
            return values;
        }

        /// <summary>
        ///   Creates a new matrix.
        /// </summary>
        /// 
        protected mat matrix(double[] values, int rows, int cols)
        {
            return Matrix.Reshape(values, rows, cols);
        }


        /// <summary>
        ///   Placeholder vector definition
        /// </summary>
        /// 
        protected vec _
        {
            get { return new vec(null); }
        }


        /// <summary>
        ///   Vector definition operator.
        /// </summary>
        /// 
        protected class vec
        {
            /// <summary>
            ///   Inner vector object
            /// </summary>
            /// 
            public double[] vector;

            /// <summary>
            ///   Initializes a new instance of the <see cref="vec"/> class.
            /// </summary>
            /// 
            public vec(double[] values)
            {
                this.vector = values;
            }

            /// <summary>
            ///   Implements the operator -.
            /// </summary>
            /// 
            public static vec operator -(vec v)
            {
                return v;
            }

            /// <summary>
            ///   Implements the operator &lt;.
            /// </summary>
            /// 
            public static vec operator <(vec a, vec v)
            {
                    a.vector = v.vector;
                    return a;
            }

            /// <summary>
            ///   Implements the operator &gt;.
            /// </summary>
            /// 
            public static vec operator >(vec a, vec v)
            {
                return a;
            }

            /// <summary>
            ///   Performs an implicit conversion from <see cref="T:System.Double[]"/>
            ///   to <see cref="Accord.Math.Environments.REnvironment.vec"/>.
            /// </summary>
            /// 
            public static implicit operator vec(double[] v)
            {
                return new vec(v);
            }


            /// <summary>
            ///   Performs an implicit conversion from 
            ///   <see cref="Accord.Math.Environments.REnvironment.vec"/> 
            ///   to <see cref="T:System.Double[]"/>.
            /// </summary>
            /// 
            public static implicit operator double[](vec v)
            {
                return v.vector;
            }
        }

        /// <summary>
        ///   Matrix definition operator.
        /// </summary>
        /// 
        protected class mat
        {
            /// <summary>
            ///   Inner matrix object.
            /// </summary>
            /// 
            public double[,] matrix;

            /// <summary>
            ///   Initializes a new instance of the <see cref="mat"/> class.
            /// </summary>
            /// 
            public mat(double[,] values)
            {
                this.matrix = values;
            }

            /// <summary>
            ///   Implements the operator -.
            /// </summary>
            /// 
            public static mat operator -(mat v)
            {
                return v;
            }

            /// <summary>
            ///   Implements the operator &lt;.
            /// </summary>
            /// 
            public static mat operator <(mat a, mat v)
            {
                a.matrix = v.matrix;
                return a;
            }

            /// <summary>
            ///    Implements the operator &gt;.
            /// </summary>
            /// 
            public static mat operator >(mat a, mat v)
            {
                return a;
            }

            /// <summary>
            ///   Performs an implicit conversion from 
            ///   <see cref="T:System.Double[]"/> to 
            ///   <see cref="Accord.Math.Environments.REnvironment.mat"/>.
            /// </summary>
            /// 
            public static implicit operator mat(double[,] v)
            {
                return new mat(v);
            }


            /// <summary>
            ///   Performs an implicit conversion from 
            ///   <see cref="Accord.Math.Environments.REnvironment.mat"/> 
            ///   to <see cref="T:System.Double[]"/>.
            /// </summary>
            /// 
            public static implicit operator double[,](mat v)
            {
                return v.matrix;
            }
        }
    }
}
