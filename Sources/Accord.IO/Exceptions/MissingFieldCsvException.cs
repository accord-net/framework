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
    using System.Globalization;
    using System.Runtime.Serialization;
    using Accord.Compat;
    using System.Security.Permissions;

    /// <summary>
    ///   Represents the exception that is thrown when a there is a missing field in a record of the CSV file.
    /// </summary>
    /// 
    /// <remarks>
    ///   MissingFieldException would have been a better name, but there is already a <see cref="T:System.MissingFieldException"/>.
    /// </remarks>
    /// 
    [Serializable()]
    public class MissingFieldCsvException
        : MalformedCsvException
    {

        /// <summary>
        ///   Initializes a new instance of the MissingFieldCsvException class.
        /// </summary>
        /// 
        public MissingFieldCsvException()
            : base()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the MissingFieldCsvException class.
        /// </summary>
        /// 
        /// <param name="message">The message that describes the error.</param>
        /// 
        public MissingFieldCsvException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the MissingFieldCsvException class.
        /// </summary>
        /// 
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// 
        public MissingFieldCsvException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the MissingFieldCsvException class.
        /// </summary>
        /// 
        /// <param name="rawData">The raw data when the error occured.</param>
        /// <param name="currentPosition">The current position in the raw data.</param>
        /// <param name="currentRecordIndex">The current record index.</param>
        /// <param name="currentFieldIndex">The current field index.</param>
        /// 
        public MissingFieldCsvException(string rawData, int currentPosition, long currentRecordIndex, int currentFieldIndex)
            : base(rawData, currentPosition, currentRecordIndex, currentFieldIndex)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the MissingFieldCsvException class.
        /// </summary>
        /// 
        /// <param name="rawData">The raw data when the error occured.</param>
        /// <param name="currentPosition">The current position in the raw data.</param>
        /// <param name="currentRecordIndex">The current record index.</param>
        /// <param name="currentFieldIndex">The current field index.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// 
        public MissingFieldCsvException(string rawData, int currentPosition, long currentRecordIndex, int currentFieldIndex, Exception innerException)
            : base(rawData, currentPosition, currentRecordIndex, currentFieldIndex, innerException)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the MissingFieldCsvException class with serialized data.
        /// </summary>
        /// 
        /// <param name="info">The <see cref="T:SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// 
        protected MissingFieldCsvException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }
}