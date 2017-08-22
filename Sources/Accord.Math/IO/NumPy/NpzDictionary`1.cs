// Accord Formats Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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

#if !NET35 && !NET40
namespace Accord.IO
{
    using Accord.Math;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.IO.Compression;
    using System.Collections;
    using System.Data;
    using Accord.Compat;

    /// <summary>
    ///   Lazily-loaded collection of arrays from a compressed .npz archive.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the arrays to be loaded.</typeparam>
    /// 
    /// <seealso cref="NpyFormat"/>
    /// <seealso cref="NpzFormat"/>
    /// 
    public class NpzDictionary<T> : IDisposable, IReadOnlyDictionary<string, T>, ICollection<T>
        where T : class,
#if !NETSTANDARD1_4
        ICloneable,
#endif
        IList, ICollection, IEnumerable, IStructuralComparable, IStructuralEquatable
    {
        Stream stream;
        ZipArchive archive;

        bool disposedValue = false;

        Dictionary<string, ZipArchiveEntry> entries;
        Dictionary<string, T> arrays;


        /// <summary>
        ///   Initializes a new instance of the <see cref="NpzDictionary{T}"/> class.
        /// </summary>
        /// 
        /// <param name="stream">The stream from where the arrays should be loaded from.</param>
        /// 
        public NpzDictionary(Stream stream)
        {
            this.stream = stream;
            this.archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true);

            this.entries = new Dictionary<string, ZipArchiveEntry>();
            foreach (var entry in archive.Entries)
                this.entries[entry.Name] = entry;

            this.arrays = new Dictionary<string, T>();
        }



        /// <summary>
        /// Gets an enumerable collection that contains the keys in the read-only dictionary.
        /// </summary>
        /// 
        /// <value>The keys.</value>
        /// 
        public IEnumerable<string> Keys
        {
            get { return entries.Keys; }
        }

        /// <summary>
        /// Gets an enumerable collection that contains the values in the read-only dictionary.
        /// </summary>
        /// 
        /// <value>The values.</value>
        /// 
        public IEnumerable<T> Values
        {
            get { return entries.Values.Select(OpenEntry); }
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        /// 
        /// <value>The count.</value>
        /// 
        public int Count
        {
            get { return entries.Count; }
        }

        /// <summary>
        ///   Gets an object that can be used to synchronize access to the collection.
        /// </summary>
        /// 
        public object SyncRoot
        {
            get { return ((ICollection)entries).SyncRoot; }
        }

        /// <summary>
        /// Gets a value indicating whether the access to collection is synchronized (thread-safe).
        /// </summary>
        /// 
        public bool IsSynchronized
        {
            get { return ((ICollection)entries).IsSynchronized; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// 
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        /// 
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        ///    Gets the array stored under the given key.
        /// </summary>
        /// 
        /// <param name="key">The key.</param>
        /// 
        public T this[string key]
        {
            get { return OpenEntry(entries[key]); }
        }



        private T OpenEntry(ZipArchiveEntry entry)
        {
            T array;
            if (arrays.TryGetValue(entry.Name, out array))
                return array;

            Stream s = entry.Open();
            array = Load(s);
            arrays[entry.Name] = array;
            return array;
        }

        /// <summary>
        ///   Loads the array from the specified stream.
        /// </summary>
        /// 
        protected virtual T Load(Stream s)
        {
            return NpyFormat.Load<T>(s);
        }

        /// <summary>
        /// Determines whether the read-only dictionary contains an element that has the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>true if the read-only dictionary contains an element that has the specified key; otherwise, false.</returns>
        public bool ContainsKey(string key)
        {
            return entries.ContainsKey(key);
        }

        /// <summary>
        /// Gets the value that is associated with the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the object that implements the <see cref="T:System.Collections.Generic.IReadOnlyDictionary`2" /> interface contains an element that has the specified key; otherwise, false.</returns>
        public bool TryGetValue(string key, out T value)
        {
            value = default(T);
            ZipArchiveEntry entry;
            if (!entries.TryGetValue(key, out entry))
                return false;
            value = OpenEntry(entry);
            return true;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            foreach (var entry in archive.Entries)
                yield return new KeyValuePair<string, T>(entry.Name, OpenEntry(entry));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var entry in archive.Entries)
                yield return new KeyValuePair<string, T>(entry.Name, OpenEntry(entry));
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            foreach (var entry in archive.Entries)
                yield return OpenEntry(entry);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(Array array, int arrayIndex)
        {
            foreach (var v in this)
                array.SetValue(v, arrayIndex++);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var v in this)
                array.SetValue(v, arrayIndex++);
        }

        /// <summary>
        ///   Throws a <see cref="ReadOnlyException"/>.
        /// </summary>
        /// 
        public void Add(T item)
        {
            throw new ReadOnlyException();
        }

        /// <summary>
        ///   Throws a <see cref="ReadOnlyException"/>.
        /// </summary>
        /// 
        public void Clear()
        {
            throw new ReadOnlyException();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.</returns>
        public bool Contains(T item)
        {
            foreach (var v in this)
                if (Object.Equals(v.Value, item))
                    return true;
            return false;
        }

        /// <summary>
        ///   Throws a <see cref="ReadOnlyException"/>.
        /// </summary>
        /// 
        public bool Remove(T item)
        {
            throw new ReadOnlyException();
        }



        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    archive.Dispose();
                    stream.Dispose();
                }

                archive = null;
                stream = null;
                entries = null;
                arrays = null;

                disposedValue = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

    }
}
#endif