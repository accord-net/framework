// Accord Video Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// Copyright © Stig Voss, 2017
// http://stigvoss.dk
// stig.voss at gmail.com
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

namespace Accord.Video
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    /// Handles functionality related to parsing a MJPEG stream
    /// </summary>
    public class MJPEGStreamParser
    {
        private const int READ_SIZE = 1024;
        private const int BUFFER_SIZE = READ_SIZE * READ_SIZE;

        private readonly byte[] _buffer;

        private int _position = 0;
        private int _totalReadBytes = 0;

        private int _imageHeaderIndex = -1;
        private int _imageBoundaryIndex = -1;

        private readonly byte[] _header;
        private readonly Boundary _boundary;

        /// <summary>
        /// Creates instance of MJPEG stream parser using a boundary and a JPEG magic header
        /// </summary>
        /// <param name="boundary"></param>
        /// <param name="header"></param>
        /// <param name="bufferSize"></param>
        public MJPEGStreamParser(Boundary boundary, byte[] header, int bufferSize = BUFFER_SIZE)
        {
            _header = header;
            _boundary = boundary;

            _buffer = new byte[bufferSize];
        }

        /// <summary>
        /// Content of byte array buffer
        /// </summary>
        public byte[] Content
        {
            get { return _buffer; }
        }

        private int RemainingBytes
        {
            get { return _totalReadBytes - _position; }
        }

        private bool HasStart
        {
            get { return _imageHeaderIndex != -1; }
        }

        private bool HasEnd
        {
            get { return _imageBoundaryIndex != -1; }
        }

        /// <summary>
        /// True if frame is detected using DetectFrame and not removed using RemoveFrame
        /// </summary>
        public bool HasFrame
        {
            get { return HasStart && HasEnd; }
        }

        private void Add(byte[] content, int readBytes)
        {
            Array.Copy(content, 0, _buffer, _totalReadBytes, readBytes);
            _totalReadBytes += readBytes;
        }

        /// <summary>
        /// Reads byte content to internal buffer from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public int Read(Stream stream)
        {
            EnsurePositionInRange();

            int readBytes;
            byte[] buffer = new byte[READ_SIZE];
            int offset = _totalReadBytes;

            readBytes = stream.Read(buffer, 0, READ_SIZE);

            if (readBytes == 0)
                throw new ApplicationException();

            Add(buffer, readBytes);

            return readBytes;
        }

        private void EnsurePositionInRange()
        {
            bool isOutOfRange = _totalReadBytes > BUFFER_SIZE - READ_SIZE;
            if (isOutOfRange)
            {
                _position = 0;
            }
        }

        /// <summary>
        /// Detects if a frame is present in the internal buffer
        /// </summary>
        public void DetectFrame()
        {
            if (!HasStart && CanRead(_header))
            {
                _imageHeaderIndex = FindHeader();

                if (HasStart)
                {
                    PositionAfterHeader();
                }
                else
                {
                    PositionAtEnd();
                }
            }

            while (HasStart && !HasEnd && CanRead(_boundary))
            {
                _imageBoundaryIndex = FindBoundary();

                if (!HasEnd)
                {
                    PositionAtEnd();
                }
            }
        }

        /// <summary>
        /// Retrieves the frame from the internal buffer
        /// </summary>
        /// <returns></returns>
        public Bitmap GetFrame()
        {
            if (HasFrame)
            {
                PositionAtImageEnd();

                int length = _imageBoundaryIndex - _imageHeaderIndex;
                Stream imageStream = new MemoryStream(_buffer, _imageHeaderIndex, length);
                return (Bitmap)Image.FromStream(imageStream);
            }
            else
            {
                throw new InvalidOperationException("No frame detected in buffer.");
            }
        }

        /// <summary>
        /// Removes current frame from buffer
        /// </summary>
        public void RemoveFrame()
        {
            if (HasFrame)
            {
                _position = _imageBoundaryIndex + _boundary.Length;
                Array.Copy(_buffer, _position, _buffer, 0, RemainingBytes);

                _totalReadBytes = RemainingBytes;
                _position = 0;

                _imageHeaderIndex = -1;
                _imageBoundaryIndex = -1;
            }
            else
            {
                throw new InvalidOperationException("No frame detected in buffer.");
            }
        }

        private void PositionAtEnd()
        {
            if (_boundary.HasValue)
            {
                int boundaryPosition = _boundary.Length - 1;
                _position = _totalReadBytes - boundaryPosition;
            }
            else
            {
                _position = _totalReadBytes;
            }
        }

        private int FindHeader()
        {
            return ByteArrayUtils.Find(_buffer, _header, _position, RemainingBytes);
        }

        private int FindBoundary()
        {
            byte[] imageDelimiter;

            if (_boundary.Length != 0)
            {
                imageDelimiter = (byte[])_boundary;
            }
            else
            {
                imageDelimiter = _header;
            }

            return ByteArrayUtils.Find(_buffer, imageDelimiter, _position, RemainingBytes);
        }

        internal int FindImageBoundary()
        {
            return ByteArrayUtils.Find(_buffer, (byte[])_boundary, 0, RemainingBytes);
        }

        private void PositionAtImageEnd()
        {
            _position = _imageBoundaryIndex;
        }

        private void PositionAfterHeader()
        {
            _position = _imageHeaderIndex + _header.Length;
        }

        internal bool CanRead(Boundary boundary)
        {
            byte[] target = (byte[])boundary;
            return CanRead(target);
        }

        internal bool CanRead(byte[] target)
        {
            return RemainingBytes != 0 && RemainingBytes >= target.Length;
        }
    }
}
