// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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

namespace Accord.Math
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    ///   Sparse vector representation (in LibSVM format).
    /// </summary>
    /// 
    /// <typeparam name="T">The type for the non-zero elements in this vector.</typeparam>
    /// 
    public class Sparse<T> : IEnumerable<T>, ICloneable,
        IList<T>, IList, IFormattable
    {
        private int[] indices;
        private T[] values;

        /// <summary>
        ///   Gets or sets the vector of indices indicating the location
        ///   of the non-zero elements contained in this sparse vector.
        /// </summary>
        /// 
        public int[] Indices
        {
            get { return indices; }
            set { indices = value; }
        }

        /// <summary>
        ///   Gets or sets the vector of values indicating which non-zero
        ///   value happens at each position indicated in <see cref="Indices"/>.
        /// </summary>
        /// 
        public T[] Values
        {
            get { return values; }
            set { values = value; }
        }

        /// <summary>
        ///   Creates a sparse vector with the maximum number of elements.
        /// </summary>
        /// 
        /// <param name="length">The maximum number of non-zero
        ///   elements that this vector can accomodate.</param>
        ///   
        public Sparse(int length)
        {
            this.indices = new int[length];
            this.values = new T[length];
        }

        /// <summary>
        ///   Creates a sparse vector from a vector of indices
        ///   and a vector of values occuring at those indices.
        /// </summary>
        /// 
        /// <param name="indices">The indices for non-zero entries.</param>
        /// <param name="values">The non-zero values happening at each index.</param>
        /// 
        public Sparse(int[] indices, T[] values)
        {
            this.indices = indices;
            this.values = values;
        }

        /// <summary>
        ///   Converts this sparse vector to a dense vector of the given length.
        /// </summary>
        /// 
        public T[] ToDense(int length)
        {
            T[] result = new T[length];
            for (int i = 0; i < Indices.Length; i++)
                result[Indices[i]] = Values[i];
            return result;
        }

        /// <summary>
        ///   Converts this sparse vector to a sparse representation where 
        ///   the indices are intertwined with their corresponding values.
        /// </summary>
        /// 
        public T[] ToSparse()
        {
            T[] result = new T[Indices.Length * 2];
            for (int i = 0; i < Indices.Length; i++)
            {
                result[2 * i + 0] = (T)System.Convert.ChangeType(Indices[i], typeof(T));
                result[2 * i + 1] = Values[i];
            }

            return result;
        }





        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public object Clone()
        {
            return new Sparse<T>((int[])indices.Clone(), (T[])values.Clone());
        }

        /// <summary>
        ///   Performs an implicit conversion from <see cref="Sparse{T}"/> to <see cref="Array"/>.
        /// </summary>
        /// 
        public static implicit operator Array(Sparse<T> obj)
        {
            return obj.Values;
        }







        int IList<T>.IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        T IList<T>.this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.Clear()
        {
            Array.Clear(values, 0, values.Length);
        }

        bool ICollection<T>.Contains(T item)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        int ICollection<T>.Count
        {
            get { return 0; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        int IList.Add(object value)
        {
            throw new NotImplementedException();
        }

        void IList.Clear()
        {
            Array.Clear(values, 0, values.Length);
        }

        bool IList.Contains(object value)
        {
            throw new NotImplementedException();
        }

        int IList.IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        void IList.Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        bool IList.IsFixedSize
        {
            get { return true; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        void IList.Remove(object value)
        {
            throw new NotImplementedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        object IList.this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        int ICollection.Count
        {
            get { return 0; }
        }

        bool ICollection.IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ToString("g", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            var sb = new StringBuilder();
            sb.Append("{");
            for (int i = 0; i < Indices.Length; i++)
            {
                sb.Append(Indices[i]);
                sb.Append(":");
                sb.AppendFormat(formatProvider, "{0:" + format + "}", Values[i]);
                if (i < Indices.Length - 1)
                    sb.Append(" ");
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
