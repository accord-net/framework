// AForge Video Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace Accord.Video
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    public class TimeoutStream : Stream
    {
        private const int DEFAULT_TIMEOUT_READ = 30000;
        private const int DEFAULT_TIMEOUT_WRITE = 30000;

        private Stream _baseStream;
#if !NET35 && !NET40
        private CancellationTokenSource _source;
#endif

        private int _readTimeout = DEFAULT_TIMEOUT_READ;
        private int _writeTimeout = DEFAULT_TIMEOUT_WRITE;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public TimeoutStream(Stream stream)
        {
            _baseStream = stream;
#if !NET35 && !NET40
            _source = new CancellationTokenSource();
#else
            throw new NotSupportedException();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public Stream BaseStream => _baseStream;

        /// <summary>
        /// 
        /// </summary>
        public override bool CanRead => _baseStream.CanRead;

        /// <summary>
        /// 
        /// </summary>
        public override bool CanSeek => _baseStream.CanSeek;

        /// <summary>
        /// 
        /// </summary>
        public override bool CanWrite => _baseStream.CanWrite;

        /// <summary>
        /// 
        /// </summary>
        public override long Length => _baseStream.Length;

        /// <summary>
        /// 
        /// </summary>
        public override bool CanTimeout => true;

        /// <summary>
        /// 
        /// </summary>
        public override int ReadTimeout
        {
            get => _readTimeout;
            set => _readTimeout = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public override int WriteTimeout
        {
            get => _writeTimeout;
            set => _writeTimeout = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public override long Position
        {
            get => _baseStream.Position;
            set => _baseStream.Position = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Flush()
        {
            _baseStream.Flush();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
#if !NET35 && !NET40
            int result;

            if (_baseStream.CanRead && !_baseStream.CanTimeout)
            {
                try
                {
                    _source.CancelAfter(_readTimeout);
                    Task<int> task = _baseStream.ReadAsync(buffer, offset, count,
                        _source.Token);
                    result = task.Result;
                }
                catch (AggregateException)
                {
                    _source = new CancellationTokenSource();
                    throw new TimeoutException("The operation timed out.");
                }
            }
            else
            {
                result = _baseStream.Read(buffer, offset, count);
            }

            return result;
#else
            throw new NotSupportedException();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _baseStream.Seek(offset, origin);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            _baseStream.SetLength(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
#if !NET35 && !NET40
            if (_baseStream.CanWrite && !_baseStream.CanTimeout)
            {
                try
                {
                    _source.CancelAfter(_readTimeout);
                    Task task = _baseStream.WriteAsync(buffer, offset, count, _source.Token);
                    task.Wait();
                }
                catch (AggregateException)
                {
                    _source = new CancellationTokenSource();
                    throw new TimeoutException("The operation timed out.");
                }
            }
            else
            {
                _baseStream.Write(buffer, offset, count);
            }
#else
            throw new NotSupportedException();
#endif
        }
    }
}
