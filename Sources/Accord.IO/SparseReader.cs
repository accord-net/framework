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
    using Accord.Compat;

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
        private List<string> descriptions = new List<string>();

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
        ///   Gets the description associated with the last read values.
        /// </summary>
        /// 
        public List<string> SampleDescriptions { get { return descriptions; } }

        /// <summary>
        ///   Obsolete. Please use <see cref="NumberOfInputs"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use NumberOfInputs instead.")]
        public int Dimensions
        {
            get { return NumberOfInputs; }
        }

        /// <summary>
        ///   Gets the number of features present in this dataset. Please 
        ///   note that, when using the sparse representation, it is not
        ///   strictly necessary to know this value.
        /// </summary>
        /// 
        public int NumberOfInputs
        {
            get
            {
                if (sampleSize <= 0)
                    sampleSize = guessSampleSize();

                return Intercept.HasValue ? sampleSize + 1 : sampleSize;
            }
        }


        private void createReader(string path)
        {
#if NETSTANDARD1_4
            this.reader = new StreamReader(new FileStream(path, FileMode.Open));
#else
            this.reader = new StreamReader(path);
#endif
        }

        private void createReader(string path, System.Text.Encoding encoding)
        {
#if NETSTANDARD1_4
            this.reader = new StreamReader(new FileStream(path, FileMode.Open), encoding);
#else
            this.reader = new StreamReader(path);
#endif
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
            createReader(path);
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
            createReader(path);
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
        /// <param name="path">The complete file path to be read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="sampleSize">The size of the feature vectors stored in the file.</param>
        /// 
        public SparseReader(String path, System.Text.Encoding encoding, int sampleSize)
        {
            createReader(path, encoding);
            this.sampleSize = sampleSize;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SparseReader"/> class.
        /// </summary>
        /// 
        /// <param name="path">The complete file path to be read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// 
        public SparseReader(String path, System.Text.Encoding encoding)
        {
            createReader(path, encoding);
            this.sampleSize = -1;
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
        public SparseReader(Stream stream, System.Text.Encoding encoding, int sampleSize)
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
        public SparseReader(Stream stream, System.Text.Encoding encoding)
        {
            this.reader = new StreamReader(stream, encoding);
            this.sampleSize = -1;
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
        ///   Reads one line from the feature file, returning the array of values 
        ///   for the sparse vector and its corresponding label.
        /// </summary>
        /// 
        /// <returns>A tuple containing the array of values in the format 
        ///  "index:value" as the first item and their corresponding label
        ///  as the second item.</returns>
        ///  
        public Tuple<string[], string> ReadLine()
        {
            string description = String.Empty;
            string line = reader.ReadLine();
            string[] data = line.Split('#');
            string[] fields = data[0].Trim().Split(' ');
            if (data.Length > 1)
                description = data[1].Trim();

            SampleDescriptions.Add(description);

            var output = fields[0];
            var values = new string[fields.Length - 1];
            Array.Copy(fields, 1, values, 0, values.Length);
            return Tuple.Create(values, output);
        }


        /// <summary>
        ///   Reads a sample from the file and returns it as a
        ///   <see cref="Sparse{T}"/> sparse vector, together with
        ///   its associated output value.
        /// </summary>
        /// 
        /// <returns>A tuple containing the sparse vector as the first item
        ///   and its associated output value as the second item.</returns>
        ///   
        public Tuple<Sparse<double>, double> ReadSparse()
        {
            Sparse<double> sample;
            double output;
            Read(out sample, out output);
            return Tuple.Create(sample, output);
        }

        /// <summary>
        ///   Reads a sample from the file and returns it as a
        ///   <see cref="Sparse{T}"/> sparse vector, together with
        ///   its associated output value.
        /// </summary>
        /// 
        /// <returns>A tuple containing the sparse vector as the first item
        ///   and its associated output value as the second item.</returns>
        ///   
        public void Read(out Sparse<double> sample, out double output)
        {
            var values = ReadLine();
            output = Double.Parse(values.Item2, System.Globalization.CultureInfo.InvariantCulture);
            sample = Sparse.Parse(values.Item1, Intercept);
        }

        /// <summary>
        ///   Reads a sample from the file and returns it as a
        ///   <see cref="Sparse{T}"/> sparse vector, together with
        ///   its associated output value.
        /// </summary>
        /// 
        /// <returns>A tuple containing the sparse vector as the first item
        ///   and its associated output value as the second item.</returns>
        ///   
        public void Read(out Sparse<double> sample, out int output)
        {
            var values = ReadLine();
            output = Int32.Parse(values.Item2, System.Globalization.CultureInfo.InvariantCulture);
            sample = Sparse.Parse(values.Item1, Intercept);
        }

        /// <summary>
        ///   Reads a sample from the file and returns it as a
        ///   <see cref="Sparse{T}"/> sparse vector, together with
        ///   its associated output value.
        /// </summary>
        /// 
        /// <returns>A tuple containing the sparse vector as the first item
        ///   and its associated output value as the second item.</returns>
        ///   
        public void Read(out Sparse<double> sample, out bool output)
        {
            var values = ReadLine();
            output = double.Parse(values.Item2) > 0;
            sample = Sparse.Parse(values.Item1, Intercept);
        }

        /// <summary>
        ///   Reads <paramref name="count"/> samples from the file and returns
        ///   them as a <see cref="Sparse{T}"/> sparse vector, together with
        ///   their associated output values.
        /// </summary>
        /// 
        /// <param name="count">The number of samples to read.</param>
        /// 
        /// <returns>A tuple containing the sparse vectors as the first item
        ///   and their associated output values as the second item.</returns>
        /// 
        public Tuple<Sparse<double>[], double[]> ReadSparse(int count)
        {
            Sparse<double>[] samples;
            double[] outputs;
            Read(count, out samples, out outputs);
            return Tuple.Create(samples, outputs);
        }

        /// <summary>
        ///   Reads <paramref name="count"/> samples from the file and returns
        ///   them as a <see cref="Sparse{T}"/> sparse vector, together with
        ///   their associated output values.
        /// </summary>
        /// 
        /// <param name="count">The number of samples to read.</param>
        /// <param name="samples">The samples that have been read from the file.</param>
        /// <param name="outputs">The output labels associated with each sample in <paramref name="samples"/>.</param>
        /// 
        /// 
        public void Read(int count, out Sparse<double>[] samples, out double[] outputs)
        {
            var sampleList = new List<Sparse<double>>();
            var outputList = new List<double>();

            while (!reader.EndOfStream)
            {
                Sparse<double> s;
                double o;
                Read(out s, out o);
                sampleList.Add(s);
                outputList.Add(o);
                if (count > 0 && sampleList.Count >= count)
                    break;
            }

            samples = sampleList.ToArray();
            outputs = outputList.ToArray();
        }


        /// <summary>
        ///   Reads <paramref name="count"/> samples from the file and returns
        ///   them as a <see cref="Sparse{T}"/> sparse vector, together with
        ///   their associated output values.
        /// </summary>
        /// 
        /// <param name="count">The number of samples to read.</param>
        /// <param name="samples">The samples that have been read from the file.</param>
        /// <param name="outputs">The output labels associated with each sample in <paramref name="samples"/>.</param>
        /// 
        /// 
        public void Read(int count, out Sparse<double>[] samples, out int[] outputs)
        {
            var sampleList = new List<Sparse<double>>();
            var outputList = new List<int>();

            while (!reader.EndOfStream)
            {
                Sparse<double> s;
                int o;
                Read(out s, out o);
                sampleList.Add(s);
                outputList.Add(o);
                if (count > 0 && sampleList.Count >= count)
                    break;
            }

            samples = sampleList.ToArray();
            outputs = outputList.ToArray();
        }

        /// <summary>
        ///   Reads <paramref name="count"/> samples from the file and returns
        ///   them as a <see cref="Sparse{T}"/> sparse vector, together with
        ///   their associated output values.
        /// </summary>
        /// 
        /// <param name="count">The number of samples to read.</param>
        /// <param name="samples">The samples that have been read from the file.</param>
        /// <param name="outputs">The output labels associated with each sample in <paramref name="samples"/>.</param>
        /// 
        public void Read(int count, out Sparse<double>[] samples, out bool[] outputs)
        {
            var sampleList = new List<Sparse<double>>();
            var outputList = new List<bool>();

            while (!reader.EndOfStream)
            {
                Sparse<double> s;
                bool o;
                Read(out s, out o);
                sampleList.Add(s);
                outputList.Add(o);
                if (count > 0 && sampleList.Count >= count)
                    break;
            }

            samples = sampleList.ToArray();
            outputs = outputList.ToArray();
        }

        /// <summary>
        ///   Reads all samples from the file and returns them as a
        ///   <see cref="Sparse{T}"/> sparse vector, together with
        ///   their associated output values.
        /// </summary>
        /// 
        /// <returns>A tuple containing the sparse vectors as the first item
        ///   and their associated output values as the second item.</returns>
        /// 
        public Tuple<Sparse<double>[], double[]> ReadSparseToEnd()
        {
            return ReadSparse(-1);
        }

        /// <summary>
        ///   Reads all samples from the file and returns them as a
        ///   <see cref="Sparse{T}"/> sparse vector, together with
        ///   their associated output values.
        /// </summary>
        /// 
        /// <param name="samples">The samples that have been read from the file.</param>
        /// <param name="outputs">The output labels associated with each sample in <paramref name="samples"/>.</param>
        /// 
        public void ReadToEnd(out Sparse<double>[] samples, out double[] outputs)
        {
            Read(-1, out samples, out outputs);
        }

        /// <summary>
        ///   Reads all samples from the file and returns them as a
        ///   <see cref="Sparse{T}"/> sparse vector, together with
        ///   their associated output values.
        /// </summary>
        /// 
        /// <param name="samples">The samples that have been read from the file.</param>
        /// <param name="outputs">The output labels associated with each sample in <paramref name="samples"/>.</param>
        /// 
        public void ReadToEnd(out Sparse<double>[] samples, out bool[] outputs)
        {
            Read(-1, out samples, out outputs);
        }

        /// <summary>
        ///   Reads all samples from the file and returns them as a
        ///   <see cref="Sparse{T}"/> sparse vector, together with
        ///   their associated output values.
        /// </summary>
        /// 
        /// <param name="samples">The samples that have been read from the file.</param>
        /// <param name="outputs">The output labels associated with each sample in <paramref name="samples"/>.</param>
        /// 
        public void ReadToEnd(out Sparse<double>[] samples, out int[] outputs)
        {
            Read(-1, out samples, out outputs);
        }


        /// <summary>
        ///   Reads a sample from the file and returns it as a
        ///   dense vector, together with its associated output value.
        /// </summary>
        /// 
        /// <returns>A tuple containing the dense vector as the first item
        ///   and its associated output value as the second item.</returns>
        ///   
        public Tuple<double[], double> ReadDense()
        {
            var sparse = ReadSparse();
            return Tuple.Create(sparse.Item1.ToDense(NumberOfInputs), sparse.Item2);
        }

        /// <summary>
        ///   Reads <paramref name="count"/> samples from the file and returns
        ///   them as a <see cref="Sparse{T}"/> sparse vector, together with
        ///   their associated output values.
        /// </summary>
        /// 
        /// <param name="count">The number of samples to read.</param>
        /// 
        /// <returns>A tuple containing the sparse vectors as the first item
        ///   and their associated output values as the second item.</returns>
        /// 
        public Tuple<double[][], double[]> ReadDense(int count)
        {
            var sparse = ReadSparse(count);
            var dense = new double[sparse.Item1.Length][];
            for (int i = 0; i < dense.Length; i++)
                dense[i] = sparse.Item1[i].ToDense(NumberOfInputs);
            return Tuple.Create(dense, sparse.Item2);
        }

        /// <summary>
        ///   Reads all samples from the file and returns them as a
        ///   dense vector, together with their associated output values.
        /// </summary>
        /// 
        /// <returns>A tuple containing the dense vectors as the first item
        ///   and their associated output values as the second item.</returns>
        /// 
        public Tuple<double[][], double[]> ReadDenseToEnd()
        {
            return ReadDense(-1);
        }







        private int guessSampleSize()
        {
            // Scan the whole file and identify
            // the largest index we can find. 

            long position = reader.GetPosition();

            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            int max = 0;

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                int lastColon = line.LastIndexOf(':');
                int lastSpace = line.LastIndexOf(' ', lastColon);

                string str = line.Substring(lastSpace, lastColon - lastSpace);
                int index = int.Parse(str, System.Globalization.CultureInfo.InvariantCulture) - 1;

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
