// Accord Formats Library
// The Accord.NET Framework
// http://accord-framework.net
//
// LumenWorks.Framework.IO.CSV.CsvReader
// Copyright (c) 2005 S�bastien Lorion
//
// Copyright � C�sar Souza, 2009-2016
// cesarsouza at gmail.com
//
// This class has been based on the original work by S�bastien Lorion, originally
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
    /// <summary>
    ///   Specifies the action to take when a parsing error has occurred.
    /// </summary>
    /// 
    public enum ParseErrorAction
    {
        /// <summary>
        ///   Raises the <see cref="M:CsvReader.ParseError"/> event.
        /// </summary>
        /// 
        RaiseEvent = 0,

        /// <summary>
        ///   Tries to advance to next line.
        /// </summary>
        /// 
        AdvanceToNextLine = 1,

        /// <summary>
        ///   Throws an exception.
        /// </summary>
        /// 
        ThrowException = 2,
    }
}
