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

namespace Accord.IO
{
    using Accord.Math;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;

    /// <summary>
    ///   Writer for data files containing samples in libsvm's sparse format.
    /// </summary>
    /// 
    public class SparseWriter : IDisposable
    {
        private StreamWriter writer;

        /// <summary>
        ///   Returns the underlying stream.
        /// </summary>
        /// 
        public Stream BaseStream
        {
            get { return writer.BaseStream; }
        }

        /// <summary>
        ///   Gets or sets whether to include an intercept term
        ///   (bias) value at the beginning of each new sample.
        ///   Default is <c>null</c> (don't include anything).
        /// </summary>
        /// 
        public double? Intercept { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SparseWriter"/> class.
        /// </summary>
        /// 
        /// <param name="path">The complete file path to be written.</param>
        /// 
        public SparseWriter(string path)
        {
            this.writer = new StreamWriter(new FileStream(path, FileMode.Create));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SparseWriter"/> class.
        /// </summary>
        /// 
        /// <param name="path">The complete file path to be written.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// 
        public SparseWriter(String path, Encoding encoding)
        {
            this.writer = new StreamWriter(new FileStream(path, FileMode.Create), encoding);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SparseWriter"/> class.
        /// </summary>
        /// 
        /// <param name="stream">The file stream to be written.</param>
        /// 
        public SparseWriter(Stream stream)
        {
            this.writer = new StreamWriter(stream);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SparseWriter"/> class.
        /// </summary>
        /// 
        /// <param name="stream">The file stream to be read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// 
        public SparseWriter(Stream stream, Encoding encoding)
        {
            this.writer = new StreamWriter(stream, encoding);
        }


        /// <summary>
        ///   Writes the given feature vectors and associated output label/value to the file.
        /// </summary>
        /// 
        /// <param name="features">The array of feature vectors to be written.</param>
        /// <param name="outputs">The array of output values to be written.</param>
        /// 
        public void Write(double[][] features, bool[] outputs)
        {
            for (int i = 0; i < features.Length; i++)
                Write(features[i], outputs[i]);
        }

        /// <summary>
        ///   Writes the given feature vectors and associated output label/value to the file.
        /// </summary>
        /// 
        /// <param name="features">The array of feature vectors to be written.</param>
        /// <param name="outputs">The array of output values to be written.</param>
        /// 
        public void Write(Sparse<double>[] features, bool[] outputs)
        {
            for (int i = 0; i < features.Length; i++)
                Write(features[i], outputs[i]);
        }

        /// <summary>
        ///   Writes the given feature vector and associated output label/value to the file.
        /// </summary>
        /// 
        /// <param name="feature">The feature vector to be written.</param>
        /// <param name="output">The output value to be written.</param>
        /// 
        public void Write(double[] feature, double output)
        {
            Write(Sparse.FromDense(feature), output);
        }

        /// <summary>
        ///   Writes the given feature vector and associated output label/value to the file.
        /// </summary>
        /// 
        /// <param name="feature">The feature vector to be written.</param>
        /// <param name="output">The output value to be written.</param>
        /// 
        public void Write(double[] feature, bool output)
        {
            Write(Sparse.FromDense(feature), output);
        }

        /// <summary>
        ///   Writes the given feature vector and associated output label/value to the file.
        /// </summary>
        /// 
        /// <param name="feature">The feature vector to be written.</param>
        /// <param name="output">The output value to be written.</param>
        /// <param name="comment">An optional comment describing the feature.</param>
        /// 
        public void Write(double[] feature, double output, string comment)
        {
            Write(Sparse.FromDense(feature), output, comment);
        }

        /// <summary>
        ///   Writes the given feature vector and associated output label/value to the file.
        /// </summary>
        /// 
        /// <param name="feature">The feature vector to be written.</param>
        /// <param name="output">The output value to be written.</param>
        /// 
        public void Write(Sparse<double> feature, double output)
        {
            writer.Write(output);
            writer.Write(" ");
            string str = feature.ToString();
            writer.Write(str);
            writer.WriteLine();
        }

        /// <summary>
        ///   Writes the given feature vector and associated output label/value to the file.
        /// </summary>
        /// 
        /// <param name="feature">The feature vector to be written.</param>
        /// <param name="output">The output value to be written.</param>
        /// <param name="comment">An optional comment describing the feature.</param>
        /// 
        public void Write(Sparse<double> feature, double output, string comment)
        {
            writer.Write(output);
            writer.Write(" ");
            writer.Write(feature.ToString());
            writer.Write(" # ");
            writer.Write(comment);
            writer.WriteLine();
        }

        /// <summary>
        ///   Writes the given feature vector and associated output label/value to the file.
        /// </summary>
        /// 
        /// <param name="feature">The feature vector to be written.</param>
        /// <param name="output">The output value to be written.</param>
        /// 
        public void Write(Sparse<double> feature, bool output)
        {
            writer.Write(output ? 1 : -1);
            writer.Write(" ");
            writer.Write(feature.ToString());
            writer.WriteLine();
        }

        /// <summary>
        ///   Writes the given feature vector and associated output label/value to the file.
        /// </summary>
        /// 
        /// <param name="feature">The feature vector to be written.</param>
        /// <param name="output">The output value to be written.</param>
        /// <param name="comment">An optional comment describing the feature.</param>
        /// 
        public void Write(Sparse<double> feature, bool output, string comment)
        {
            writer.Write(output ? 1 : -1);
            writer.Write(" ");
            writer.Write(feature.ToString());
            writer.Write(" # ");
            writer.Write(comment);
            writer.WriteLine();
        }



        #region IDisposable members
        /// <summary>
        ///   Performs application-defined tasks associated with
        ///   freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// 
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged
        ///   resources; <c>false</c> to release only unmanaged resources.</param>
        /// 
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (writer != null)
                {
                    writer.Dispose();
                    writer = null;
                }
            }
        }

        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations before the
        ///   <see cref="SparseReader"/> is reclaimed by garbage collection.
        /// </summary>
        /// 
        ~SparseWriter()
        {
            Dispose(false);
        }
        #endregion

    }
}
