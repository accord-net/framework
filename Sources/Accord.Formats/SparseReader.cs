// Accord Statistics Library
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

namespace Accord.IO
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;

    /// <summary>
    ///   Reader for data files containing samples in libsvm's sparse format.
    /// </summary>
    /// 
    /// <example>
    ///   <para>
    ///   The following example shows how to read all sparse samples from a file
    ///   and retrieve them as a dense multidimensional vector.</para>
    ///   
    ///   <code>
    ///   // Suppose we are going to read a sparse sample file containing
    ///   //  samples which have an actual dimension of 4. Since the samples
    ///   //  are in a sparse format, each entry in the file will probably
    ///   //  have a much lesser number of elements.
    ///   //
    ///   int sampleSize = 4;
    ///
    ///   // Create a new Sparse Sample Reader to read any given file,
    ///   //  passing the correct dense sample size in the constructor
    ///   //
    ///   SparseReader reader = new SparseReader(file, Encoding.Default, sampleSize);
    ///   
    ///   // Declare a vector to obtain the label
    ///   //  of each of the samples in the file
    ///   //
    ///   int[] labels = null;
    ///   
    ///   // Declare a vector to obtain the description (or comments)
    ///   //  about each of the samples in the file, if present.
    ///   //
    ///   string[] descriptions = null;
    ///   
    ///   // Read the sparse samples and store them in a dense vector array
    ///   double[][] samples = reader.ReadToEnd(out labels, out descriptions);
    ///   </code>
    ///   
    ///   <para>Additionally, it is also possible to read each sample
    ///   individually and sequentially. For this, we can use a while
    ///   loop until we reach the end of the stream.</para>
    ///   
    ///   <code>
    ///   // Suppose we are going to read a sparse sample file containing
    ///   //  samples which have an actual dimension of 4. Since the samples
    ///   //  are in a sparse format, each entry in the file will probably
    ///   //  have a much lesser number of elements.
    ///   //
    ///   int sampleSize = 4;
    ///
    ///   // Create a new Sparse Sample Reader to read any given file,
    ///   //  passing the correct dense sample size in the constructor
    ///   //
    ///   SparseReader reader = new SparseReader(file, Encoding.Default, sampleSize);
    ///
    ///   // Declare some variables to receive each sample
    ///   //
    ///   int label = 0;
    ///   string description;
    ///   double[] sample;
    ///   
    ///   // Read a single sample from the file
    ///   sample = reader.ReadDense(out label, out description);
    ///   
    ///   // Read all other samples from the file
    ///   while (!reader.EndOfStream)
    ///   {
    ///       sample = reader.ReadDense(out label, out description);
    ///   }
    ///   </code>
    /// </example>
    /// 
    public class SparseReader : IDisposable
    {
        private StreamReader reader;
        private int sampleSize; // feature vector length

        /// <summary>
        ///   Returns the underlying stream.
        /// </summary>
        /// 
        public Stream BaseStream
        {
            get { return reader.BaseStream; }
        }

        /// <summary>
        ///   Gets or sets whether to include an intercept term
        ///   (bias) value at the beginning of each new sample.
        ///   Default is <c>null</c> (don't include anything).
        /// </summary>
        /// 
        public double? Intercept { get; set; }

        /// <summary>
        ///   Gets the number of features present in this dataset. Please 
        ///   note that, when using the sparse representation, it is not
        ///   strictly necessary to know this value.
        /// </summary>
        /// 
        public int Dimensions
        {
            get
            {
                if (sampleSize <= 0)
                    sampleSize = guessSampleSize();

                return Intercept.HasValue ? sampleSize + 1 : sampleSize;
            }
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="SparseReader"/> class.
        /// </summary>
        /// 
        /// <param name="path">The complete file path to be read.</param>
        /// <param name="sampleSize">The size of the feature vectors stored in the file.</param>
        /// 
        public SparseReader(string path, int sampleSize)
        {
            this.reader = new StreamReader(path);
            this.sampleSize = sampleSize;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SparseReader"/> class.
        /// </summary>
        /// 
        /// <param name="path">The complete file path to be read.</param>
        /// 
        public SparseReader(string path)
        {
            this.reader = new StreamReader(path);
            this.sampleSize = -1;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SparseReader"/> class.
        /// </summary>
        /// 
        /// <param name="stream">The file stream to be read.</param>
        /// <param name="sampleSize">The size of the feature vectors stored in the file.</param>
        /// 
        public SparseReader(Stream stream, int sampleSize)
        {
            this.reader = new StreamReader(stream);
            this.sampleSize = sampleSize;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SparseReader"/> class.
        /// </summary>
        /// 
        /// <param name="stream">The file stream to be read.</param>
        /// 
        public SparseReader(Stream stream)
        {
            this.reader = new StreamReader(stream);
            this.sampleSize = -1;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SparseReader"/> class.
        /// </summary>
        /// 
        /// <param name="stream">The file stream to be read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="sampleSize">The size of the feature vectors stored in the file.</param>
        /// 
        public SparseReader(Stream stream, Encoding encoding, int sampleSize)
        {
            this.reader = new StreamReader(stream, encoding);
            this.sampleSize = sampleSize;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SparseReader"/> class.
        /// </summary>
        /// 
        /// <param name="stream">The file stream to be read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// 
        public SparseReader(Stream stream, Encoding encoding)
        {
            this.reader = new StreamReader(stream, encoding);
            this.sampleSize = -1;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SparseReader"/> class.
        /// </summary>
        /// 
        /// <param name="path">The complete file path to be read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="sampleSize">The size of the feature vectors stored in the file.</param>
        /// 
        public SparseReader(String path, Encoding encoding, int sampleSize)
        {
            this.reader = new StreamReader(path, encoding);
            this.sampleSize = sampleSize;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SparseReader"/> class.
        /// </summary>
        /// 
        /// <param name="reader">A StreamReader containing the file to be read.</param>
        /// <param name="sampleSize">The size of the feature vectors stored in the file.</param>
        /// 
        public SparseReader(StreamReader reader, int sampleSize)
        {
            this.reader = reader;
            this.sampleSize = sampleSize;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SparseReader"/> class.
        /// </summary>
        /// 
        /// <param name="path">The complete file path to be read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// 
        public SparseReader(String path, Encoding encoding)
        {
            this.reader = new StreamReader(path, encoding);
            this.sampleSize = -1;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SparseReader"/> class.
        /// </summary>
        /// 
        /// <param name="reader">A StreamReader containing the file to be read.</param>
        /// 
        public SparseReader(StreamReader reader)
        {
            this.reader = reader;
            this.sampleSize = -1;
        }

        /// <summary>
        ///   Gets a value that indicates whether the current
        ///   stream position is at the end of the stream.
        /// </summary>
        /// 
        public bool EndOfStream
        {
            get { return reader.EndOfStream; }
        }

        /// <summary>
        ///   Reads a sparse sample from the current stream
        ///   and returns it as a sparse vector.
        /// </summary>
        /// 
        /// <param name="label">The label of the sample.</param>
        /// <param name="description">An optional description accompanying the sample.</param>
        /// <returns>A vector in sparse representation containing the sample.</returns>
        /// 
        public double[] ReadSparse(out int label, out string description)
        {
            return read(true, out label, out description);
        }

        /// <summary>
        ///   Reads a sparse sample from the current stream
        ///   and returns it as a sparse vector.
        /// </summary>
        /// 
        /// <param name="label">The label of the sample.</param>
        /// <returns>A vector in sparse representation containing the sample.</returns>
        /// 
        public double[] ReadSparse(out int label)
        {
            string description;
            return read(true, out label, out description);
        }

        /// <summary>
        ///   Reads a sparse sample from the current stream
        ///   and returns it as a dense vector.
        /// </summary>
        /// 
        /// <param name="label">The label of the sample.</param>
        /// <param name="description">An optional description accompanying the sample.</param>
        /// 
        /// <returns>A vector in dense representation containing the sample.</returns>
        /// 
        public double[] ReadDense(out int label, out string description)
        {
            return read(false, out label, out description);
        }

        /// <summary>
        ///   Reads a sparse sample from the current stream
        ///   and returns it as a dense vector.
        /// </summary>
        /// 
        /// <param name="output">The output value associated with the sample.</param>
        /// <param name="description">An optional description accompanying the sample.</param>
        /// 
        /// <returns>A vector in dense representation containing the sample.</returns>
        /// 
        public double[] ReadDense(out double output, out string description)
        {
            return read(false, out output, out description);
        }

        /// <summary>
        ///   Reads a sparse sample from the current stream
        ///   and returns it as a dense vector.
        /// </summary>
        /// 
        /// <param name="label">The label of the sample.</param>
        /// <returns>A vector in dense representation containing the sample.</returns>
        /// 
        public double[] ReadDense(out int label)
        {
            string description;
            return read(false, out label, out description);
        }

        /// <summary>
        ///   Reads samples from the current position to the end of the stream.
        /// </summary>
        /// 
        /// <param name="labels">An array containing the samples' labels.</param>
        /// 
        /// <returns>An array of dense feature vectors.</returns>
        /// 
        public double[][] ReadToEnd(out int[] labels)
        {
            string[] descriptions;
            return ReadToEnd(false, out labels, out descriptions);
        }

        /// <summary>
        ///   Reads samples from the current position to the end of the stream.
        /// </summary>
        /// 
        /// <param name="outputs">An array containing the samples' output values.</param>
        /// 
        /// <returns>An array of dense feature vectors.</returns>
        /// 
        public double[][] ReadToEnd(out double[] outputs)
        {
            string[] descriptions;
            return ReadToEnd(false, out outputs, out descriptions);
        }

        /// <summary>
        ///   Reads samples from the current position to the end of the stream.
        /// </summary>
        /// 
        /// <param name="labels">An array containing the samples' labels.</param>
        /// <param name="descriptions">An array containing the samples' descriptions.</param>
        /// 
        /// <returns>An array of dense feature vectors.</returns>
        /// 
        public double[][] ReadToEnd(out int[] labels, out string[] descriptions)
        {
            return ReadToEnd(false, out labels, out descriptions);
        }

        /// <summary>
        ///   Reads samples from the current position to the end of the stream.
        /// </summary>
        /// 
        /// <param name="outputs">An array containing the samples' output values.</param>
        /// <param name="descriptions">An array containing the samples' descriptions.</param>
        /// 
        /// <returns>An array of dense feature vectors.</returns>
        /// 
        public double[][] ReadToEnd(out double[] outputs, out string[] descriptions)
        {
            return ReadToEnd(false, out outputs, out descriptions);
        }

        /// <summary>
        ///   Reads samples from the current position to the end of the stream.
        /// </summary>
        /// 
        /// <param name="sparse">True to return the feature vectors in a
        /// sparse representation, false to return them as dense vectors.</param>
        /// <param name="labels">An array containing the samples' labels.</param>
        /// 
        /// <returns>An array of dense feature vectors.</returns>
        /// 
        public double[][] ReadToEnd(bool sparse, out int[] labels)
        {
            string[] descriptions;
            return ReadToEnd(sparse, out labels, out descriptions);
        }

        /// <summary>
        ///   Reads samples from the current position to the end of the stream.
        /// </summary>
        /// 
        /// <param name="sparse">True to return the feature vectors in a
        /// sparse representation, false to return them as dense vectors.</param>
        /// <param name="outputs">An array containing the samples' output values.</param>
        /// 
        /// <returns>An array of dense feature vectors.</returns>
        /// 
        public double[][] ReadToEnd(bool sparse, out double[] outputs)
        {
            string[] descriptions;
            return ReadToEnd(sparse, out outputs, out descriptions);
        }

        /// <summary>
        ///   Reads samples from the current position to the end of the stream.
        /// </summary>
        /// 
        /// <param name="sparse">True to return the feature vectors in a
        /// sparse representation, false to return them as dense vectors.</param>
        /// <param name="labels">An array containing the samples' labels.</param>
        /// <param name="descriptions">An array containing the samples' descriptions.</param>
        /// 
        /// <returns>An array of dense feature vectors.</returns>
        /// 
        public double[][] ReadToEnd(bool sparse, out int[] labels, out string[] descriptions)
        {
            List<double[]> samples = new List<double[]>();
            List<int> listLabels = new List<int>();
            List<string> listDescriptions = new List<string>();

            while (!reader.EndOfStream)
            {
                int label; string description;
                samples.Add(read(sparse, out label, out description));
                listLabels.Add(label); listDescriptions.Add(description);
            }

            labels = listLabels.ToArray();
            descriptions = listDescriptions.ToArray();

            return samples.ToArray();
        }

        /// <summary>
        ///   Reads samples from the current position to the end of the stream.
        /// </summary>
        /// 
        /// <param name="sparse">True to return the feature vectors in a
        /// sparse representation, false to return them as dense vectors.</param>
        /// <param name="labels">An array containing the samples' labels.</param>
        /// <param name="descriptions">An array containing the samples' descriptions.</param>
        /// 
        /// <returns>An array of dense feature vectors.</returns>
        /// 
        public double[][] ReadToEnd(bool sparse, out double[] labels, out string[] descriptions)
        {
            List<double[]> samples = new List<double[]>();
            List<double> listLabels = new List<double>();
            List<string> listDescriptions = new List<string>();

            while (!reader.EndOfStream)
            {
                double label; string description;
                samples.Add(read(sparse, out label, out description));
                listLabels.Add(label); listDescriptions.Add(description);
            }

            labels = listLabels.ToArray();
            descriptions = listDescriptions.ToArray();

            return samples.ToArray();
        }


        private double[] read(bool sparse, out int label, out string description)
        {
            string[] values = read(out description);
            label = int.Parse(values[0], CultureInfo.InvariantCulture);
            return readFeature(sparse, values);
        }

        private double[] read(bool sparse, out double label, out string description)
        {
            string[] values = read(out description);
            label = Double.Parse(values[0], CultureInfo.InvariantCulture);
            return readFeature(sparse, values);
        }

        private string[] read(out string description)
        {
            string line = reader.ReadLine();

            string[] data = line.Split('#');
            string sample = data[0].Trim();

            description = (data.Length > 1) ? data[1].Trim() : String.Empty;

            return sample.Split(' ');
        }

        private double[] readFeature(bool sparse, string[] values)
        {
            double[] features;

            int offset = this.Intercept.HasValue ? 1 : 0;

            if (!sparse)
            {
                if (sampleSize <= 0)
                    sampleSize = guessSampleSize();

                features = new double[sampleSize + offset];
                for (int i = 1; i < values.Length; i++)
                {
                    string[] element = values[i].Split(':');
                    var index = Int32.Parse(element[0], CultureInfo.InvariantCulture) - 1;
                    var value = Double.Parse(element[1], CultureInfo.InvariantCulture);

                    features[index + offset] = value;
                }

                if (Intercept.HasValue)
                    features[0] = Intercept.Value;
            }
            else
            {
                features = new double[(values.Length - 1) * 2];
                for (int i = 1; i < values.Length; i++)
                {
                    string[] element = values[i].Split(':');
                    var index = Int32.Parse(element[0], CultureInfo.InvariantCulture) - 1;
                    var value = Double.Parse(element[1], CultureInfo.InvariantCulture);

                    int j = (i - 1) * 2;
                    features[j] = index + offset;
                    features[j + 1] = value;

                    if (index >= sampleSize)
                        sampleSize = index + 1;
                }

                if (Intercept.HasValue)
                    features[1] = Intercept.Value;
            }

            return features;
        }

        private int guessSampleSize()
        {
            // Scan the whole file and identify
            // the largest index we can find. 

            long position = reader.GetPosition();

            int max = 0;

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                int lastColon = line.LastIndexOf(':');
                int lastSpace = line.LastIndexOf(' ', lastColon);

                string str = line.Substring(lastSpace, lastColon - lastSpace);
                int index = int.Parse(str, CultureInfo.InvariantCulture) - 1;

                if (index >= max)
                    max = index + 1;
            }

            // rewind the stream to where we found it
            reader.BaseStream.Seek(position, SeekOrigin.Begin);

            return max;
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
                if (reader != null)
                {
                    reader.Dispose();
                    reader = null;
                }
            }
        }

        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations before the
        ///   <see cref="SparseReader"/> is reclaimed by garbage collection.
        /// </summary>
        /// 
        ~SparseReader()
        {
            Dispose(false);
        }
        #endregion

    }
}
