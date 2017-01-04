// Accord Formats Library
// The Accord.NET Framework
// http://accord-framework.net
//
// LumenWorks.Framework.IO.CSV.CsvReader
// Copyright (c) 2005 Sébastien Lorion
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// This class has been based on the original work by Sébastien Lorion, originally
// published under the MIT license (and thus compatible with the LGPL). Original
// license text is reproduced below:
//
//    MIT license (http://en.wikipedia.org/wiki/MIT_License)
//
//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights 
//    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//    of the Software, and to permit persons to whom the Software is furnished to do so, 
//    subject to the following conditions:
//
//    The above copyright notice and this permission notice shall be included in all 
//    copies or substantial portions of the Software.
//
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//    INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
//    PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
//    FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//    ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace Accord.IO
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Accord.IO.Resources;

	public partial class CsvReader
	{
		/// <summary>
		/// Supports a simple iteration over the records of a <see cref="T:CsvReader"/>.
		/// </summary>
		public struct RecordEnumerator
			: IEnumerator<string[]>, IEnumerator
		{
			#region Fields

			/// <summary>
			/// Contains the enumerated <see cref="T:CsvReader"/>.
			/// </summary>
			private CsvReader _reader;

			/// <summary>
			/// Contains the current record.
			/// </summary>
			private string[] _current;

			/// <summary>
			/// Contains the current record index.
			/// </summary>
			private long _currentRecordIndex;

			#endregion

			#region Constructors

			/// <summary>
			/// Initializes a new instance of the <see cref="T:RecordEnumerator"/> class.
			/// </summary>
			/// <param name="reader">The <see cref="T:CsvReader"/> to iterate over.</param>
			/// <exception cref="T:ArgumentNullException">
			///		<paramref name="reader"/> is a <see langword="null"/>.
			/// </exception>
			public RecordEnumerator(CsvReader reader)
			{
				if (reader == null)
					throw new ArgumentNullException("reader");

				_reader = reader;
				_current = null;

				_currentRecordIndex = reader._currentRecordIndex;
			}

			#endregion

			#region IEnumerator<string[]> Members

			/// <summary>
			/// Gets the current record.
			/// </summary>
			public string[] Current
			{
				get { return _current; }
			}

			/// <summary>
			/// Advances the enumerator to the next record of the CSV.
			/// </summary>
			/// <returns><see langword="true"/> if the enumerator was successfully advanced to the next record, <see langword="false"/> if the enumerator has passed the end of the CSV.</returns>
			public bool MoveNext()
			{
				if (_reader._currentRecordIndex != _currentRecordIndex)
					throw new InvalidOperationException(ExceptionMessage.EnumerationVersionCheckFailed);

				if (_reader.ReadNextRecord())
				{
					_current = new string[_reader._fieldCount];

					_reader.CopyCurrentRecordTo(_current);
					_currentRecordIndex = _reader._currentRecordIndex;

					return true;
				}
				else
				{
					_current = null;
					_currentRecordIndex = _reader._currentRecordIndex;

					return false;
				}
			}

			#endregion

			#region IEnumerator Members

			/// <summary>
			/// Sets the enumerator to its initial position, which is before the first record in the CSV.
			/// </summary>
			public void Reset()
			{
				if (_reader._currentRecordIndex != _currentRecordIndex)
					throw new InvalidOperationException(ExceptionMessage.EnumerationVersionCheckFailed);

				_reader.MoveTo(-1);

				_current = null;
				_currentRecordIndex = _reader._currentRecordIndex;
			}

			/// <summary>
			/// Gets the current record.
			/// </summary>
			object IEnumerator.Current
			{
				get
				{
					if (_reader._currentRecordIndex != _currentRecordIndex)
						throw new InvalidOperationException(ExceptionMessage.EnumerationVersionCheckFailed);

					return this.Current;
				}
			}

			#endregion

			#region IDisposable Members

			/// <summary>
			/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
			/// </summary>
			public void Dispose()
			{
				_reader = null;
				_current = null;
			}

			#endregion
		}
	}
}