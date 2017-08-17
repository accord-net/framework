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
    using Accord.IO.Resources;
    using Accord.Compat;
    using System.Security.Permissions;

    /// <summary>
    ///   Represents the exception that is thrown when a CSV file is malformed.
    /// </summary>
    /// 
    [Serializable]
    public class MalformedCsvException : Exception
    {
        string message;

        /// <summary>
        ///   Initializes a new instance of the MalformedCsvException class.
        /// </summary>
        /// 
        public MalformedCsvException()
            : this(null, null)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the MalformedCsvException class.
        /// </summary>
        /// 
        /// <param name="message">The message that describes the error.</param>
        /// 
        public MalformedCsvException(string message)
            : this(message, null)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the MalformedCsvException class.
        /// </summary>
        /// 
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// 
        public MalformedCsvException(string message, Exception innerException)
            : base(String.Empty, innerException)
        {
            message = (message == null ? string.Empty : message);

            RawData = string.Empty;
            CurrentPosition = -1;
            CurrentRecordIndex = -1;
            CurrentFieldIndex = -1;
        }

        /// <summary>
        ///   Initializes a new instance of the MalformedCsvException class.
        /// </summary>
        /// 
        /// <param name="rawData">The raw data when the error occured.</param>
        /// <param name="currentPosition">The current position in the raw data.</param>
        /// <param name="currentRecordIndex">The current record index.</param>
        /// <param name="currentFieldIndex">The current field index.</param>
        /// 
        public MalformedCsvException(string rawData, int currentPosition, long currentRecordIndex, int currentFieldIndex)
            : this(rawData, currentPosition, currentRecordIndex, currentFieldIndex, null)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the MalformedCsvException class.
        /// </summary>
        /// 
        /// <param name="rawData">The raw data when the error occured.</param>
        /// <param name="currentPosition">The current position in the raw data.</param>
        /// <param name="currentRecordIndex">The current record index.</param>
        /// <param name="currentFieldIndex">The current field index.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// 
        public MalformedCsvException(string rawData, int currentPosition, long currentRecordIndex, int currentFieldIndex, Exception innerException)
            : base(String.Empty, innerException)
        {
            RawData = (rawData == null ? string.Empty : rawData);
            CurrentPosition = currentPosition;
            CurrentRecordIndex = currentRecordIndex;
            CurrentFieldIndex = currentFieldIndex;

            message = String.Format(System.Globalization.CultureInfo.InvariantCulture, ExceptionMessage.MalformedCsvException,
                CurrentRecordIndex, CurrentFieldIndex, CurrentPosition, RawData);
        }

#if !NETSTANDARD1_4
        /// <summary>
        /// Initializes a new instance of the MalformedCsvException class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected MalformedCsvException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            message = info.GetString("MyMessage");

            RawData = info.GetString("RawData");
            CurrentPosition = info.GetInt32("CurrentPosition");
            CurrentRecordIndex = info.GetInt64("CurrentRecordIndex");
            CurrentFieldIndex = info.GetInt32("CurrentFieldIndex");
        }
#endif

        /// <summary>
        ///   Gets the raw data when the error occurred.
        /// </summary>
        /// 
        /// <value>The raw data when the error occurred.</value>
        /// 
        public string RawData { get; private set; }

        /// <summary>
        ///   Gets the current position in the raw data.
        /// </summary>
        /// 
        /// <value>The current position in the raw data.</value>
        /// 
        public int CurrentPosition { get; private set; }

        /// <summary>
        ///   Gets the current record index.
        /// </summary>
        /// 
        /// <value>The current record index.</value>
        /// 
        public long CurrentRecordIndex { get; private set; }

        /// <summary>
        ///   Gets the current field index.
        /// </summary>
        /// 
        /// <value>The current record index.</value>
        /// 
        public int CurrentFieldIndex { get; private set; }

        /// <summary>
        ///   Gets a message that describes the current exception.
        /// </summary>
        /// 
        /// <value>A message that describes the current exception.</value>
        /// 
        public override string Message { get { return message; } }

#if !NETSTANDARD1_4
        /// <summary>
        ///   When overridden in a derived class, sets the <see cref="T:SerializationInfo"/> with information about the exception.
        /// </summary>
        /// 
        /// <param name="info">The <see cref="T:SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// 
#if NET35
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
#endif
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("MyMessage", Message);

            info.AddValue("RawData", RawData);
            info.AddValue("CurrentPosition", CurrentPosition);
            info.AddValue("CurrentRecordIndex", CurrentRecordIndex);
            info.AddValue("CurrentFieldIndex", CurrentFieldIndex);
        }
#endif
    }
}