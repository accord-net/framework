// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Diego Catalano, 2013
// diego.catalano at live.com
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

namespace Accord.Imaging
{

    /// <summary>
    ///   Standard feature descriptor for <see cref="T:double[]" /> feature vectors.
    /// </summary>
    /// 
    public struct FeatureDescriptor : IFeatureDescriptor<double[]>
    {
        private double[] descriptor;

        /// <summary>
        ///   Gets or sets the descriptor vector
        ///   associated with this point.
        /// </summary>
        /// 
        public double[] Descriptor
        {
            get { return descriptor; }
            set { descriptor = value; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="FeatureDescriptor"/> structure.
        /// </summary>
        /// 
        /// <param name="value">The feature vector.</param>
        /// 
        public FeatureDescriptor(double[] value)
        {
            descriptor = value;
        }

        /// <summary>
        ///   Performs an implicit conversion from <see cref="T:double[]"/> 
        ///   to <see cref="Accord.Imaging.FeatureDescriptor"/>.
        /// </summary>
        /// 
        /// <param name="value">The value to be converted.</param>
        /// 
        /// <returns>
        ///   The result of the conversion.
        /// </returns>
        /// 
        public static implicit operator FeatureDescriptor(double[] value)
        {
            return new FeatureDescriptor(value);
        }

        /// <summary>
        ///   Performs a conversion from <see cref="T:double[]"/> 
        ///   to <see cref="Accord.Imaging.FeatureDescriptor"/>.
        /// </summary>
        /// 
        public static FeatureDescriptor FromArray(double[] value)
        {
            return new FeatureDescriptor(value);
        }

        /// <summary>
        ///   Performs an implicit conversion from <see cref="T:double[]"/> 
        ///   to <see cref="Accord.Imaging.FeatureDescriptor"/>.
        /// </summary>
        /// 
        /// <param name="value">The value to be converted.</param>
        /// 
        /// <returns>
        ///   The result of the conversion.
        /// </returns>
        /// 
        public static implicit operator FeatureDescriptor(FeatureDescriptor<double[]> value)
        {
            return new FeatureDescriptor(value.Descriptor);
        }

        /// <summary>
        ///   Performs a conversion from <see cref="T:double[]" /> 
        ///   to <see cref="Accord.Imaging.FeatureDescriptor"/>.
        /// </summary>
        /// 
        public static FeatureDescriptor FromGeneric(FeatureDescriptor<double[]> value)
        {
            return new FeatureDescriptor(value.Descriptor);
        }

        /// <summary>
        ///   Performs a conversion from <typeparam name="T"/>
        ///   to <see cref="Accord.Imaging.FeatureDescriptor{T}"/>.
        /// </summary>
        /// 
        public static FeatureDescriptor<T> FromValue<T>(T value)
        {
            return new FeatureDescriptor<T>(value);
        }

        /// <summary>
        ///   Implements the operator ==.
        /// </summary>
        /// 
        public static bool operator ==(FeatureDescriptor desc1, FeatureDescriptor desc2)
        {
            return desc1.descriptor == desc2;
        }

        /// <summary>
        ///   Implements the operator !=.
        /// </summary>
        /// 
        public static bool operator !=(FeatureDescriptor desc1, FeatureDescriptor desc2)
        {
            return desc1.descriptor != desc2;
        }

        /// <summary>
        ///   Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// 
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// 
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// 
        public override bool Equals(object obj)
        {
            if (!(obj is FeatureDescriptor))
                return false;
            return descriptor == ((FeatureDescriptor)obj).descriptor;
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing
        ///   algorithms and data structures like a hash table. 
        /// </returns>
        /// 
        public override int GetHashCode()
        {
            return descriptor.GetHashCode();
        }
    }


    /// <summary>
    ///   Standard feature descriptor for generic feature vectors.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of feature vector, such as <see cref="T:double[]"/>.</typeparam>
    /// 
    public struct FeatureDescriptor<T> : IFeatureDescriptor<T>
    {
        private T descriptor;

        /// <summary>
        ///   Gets or sets the descriptor vector
        ///   associated with this point.
        /// </summary>
        /// 
        public T Descriptor
        {
            get { return descriptor; }
            set { descriptor = value; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="FeatureDescriptor"/> struct.
        /// </summary>
        /// 
        /// <param name="value">The feature vector.</param>
        /// 
        public FeatureDescriptor(T value)
        {
            descriptor = value;
        }

        /// <summary>
        ///   Performs an implicit conversion from <typeparamref name="T"/>
        ///   to <see cref="Accord.Imaging.FeatureDescriptor"/>.
        /// </summary>
        /// 
        /// <param name="value">The value to be converted.</param>
        /// 
        /// <returns>
        ///   The result of the conversion.
        /// </returns>
        /// 
        public static implicit operator FeatureDescriptor<T>(T value)
        {
            return new FeatureDescriptor<T>(value);
        }

        /// <summary>
        ///   Implements the operator ==.
        /// </summary>
        /// 
        public static bool operator ==(FeatureDescriptor<T> desc1, FeatureDescriptor<T> desc2)
        {
            return desc1.descriptor == desc2;
        }

        /// <summary>
        ///   Implements the operator !=.
        /// </summary>
        /// 
        public static bool operator !=(FeatureDescriptor<T> desc1, FeatureDescriptor<T> desc2)
        {
            return desc1.descriptor != desc2;
        }

        /// <summary>
        ///   Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// 
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// 
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// 
        public override bool Equals(object obj)
        {
            if (!(obj is FeatureDescriptor))
                return false;
            return descriptor.Equals(((FeatureDescriptor<T>)obj).descriptor);
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing
        ///   algorithms and data structures like a hash table. 
        /// </returns>
        /// 
        public override int GetHashCode()
        {
            return descriptor.GetHashCode();
        }
    }
}
