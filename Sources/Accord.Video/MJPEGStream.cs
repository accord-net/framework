// Accord Video Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Stig Voss, 2017
// http://stigvoss.dk
// stig.voss at gmail.com
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// AForge Video Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2012
// contacts@aforgenet.com
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
    using System.Text;
    using System.Threading;
    using System.Net;
    using System.Security;

    /// <summary>
    /// MJPEG video source.
    /// </summary>
    /// 
    /// <remarks><para>The video source downloads JPEG images from the specified URL, which represents
    /// MJPEG stream.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create MJPEG video source
    /// MJPEGStream stream = new MJPEGStream( "some url" );
    /// // set event handlers
    /// stream.NewFrame += new NewFrameEventHandler( video_NewFrame );
    /// // start the video source
    /// stream.Start( );
    /// // ...
    /// </code>
    /// 
    /// <para><note>Some cameras produce HTTP header, which does not conform strictly to
    /// standard, what leads to .NET exception. To avoid this exception the <b>useUnsafeHeaderParsing</b>
    /// configuration option of <b>httpWebRequest</b> should be set, what may be done using application
    /// configuration file.</note></para>
    /// <code>
    /// &lt;configuration&gt;
    /// 	&lt;system.net&gt;
    /// 		&lt;settings&gt;
    /// 			&lt;httpWebRequest useUnsafeHeaderParsing="true" /&gt;
    /// 		&lt;/settings&gt;
    /// 	&lt;/system.net&gt;
    /// &lt;/configuration&gt;
    /// </code>
    /// </remarks>
    /// 
    public partial class MJPEGStream : IVideoSource
    {
        // URL for MJPEG stream
        private string _source;
        // login and password for HTTP authentication
        private string _userName = null;
        private string _password = null;
        // proxy information
        private IWebProxy _proxy = null;
        // received frames count
        private int _framesReceived;
        // recieved byte count
        private long _bytesReceived;
        // use separate HTTP connection group or use default
        private bool _useSeparateConnectionGroup = true;
        // timeout value for web request
        private int _requestTimeout = 10000;
        // if we should use basic authentication when connecting to the video source
        private bool _forceBasicAuthentication = false;

        private Thread _thread = null;
        private ManualResetEvent _stopEvent = null;
        private ManualResetEvent _reloadEvent = null;

        private string _userAgent = "Mozilla/5.0";

        // JPEG Magic Header
        private readonly static byte[] JPEG_HEADER_BYTES = new byte[] { 0xFF, 0xD8, 0xFF };

        /// <summary>
        /// New frame event.
        /// </summary>
        /// 
        /// <remarks><para>Notifies clients about new available frame from video source.</para>
        /// 
        /// <para><note>Since video source may have multiple clients, each client is responsible for
        /// making a copy (cloning) of the passed video frame, because the video source disposes its
        /// own original copy after notifying of clients.</note></para>
        /// </remarks>
        /// 
        public event NewFrameEventHandler NewFrame;

        /// <summary>
        /// Video source error event.
        /// </summary>
        /// 
        /// <remarks>This event is used to notify clients about any type of errors occurred in
        /// video source object, for example internal exceptions.</remarks>
        /// 
        public event VideoSourceErrorEventHandler VideoSourceError;

        /// <summary>
        /// Video playing finished event.
        /// </summary>
        /// 
        /// <remarks><para>This event is used to notify clients that the video playing has finished.</para>
        /// </remarks>
        /// 
        public event PlayingFinishedEventHandler PlayingFinished;

        /// <summary>
        /// Use or not separate connection group.
        /// </summary>
        /// 
        /// <remarks>The property indicates to open web request in separate connection group.</remarks>
        /// 
        public bool SeparateConnectionGroup
        {
            get { return _useSeparateConnectionGroup; }
            set { _useSeparateConnectionGroup = value; }
        }

        /// <summary>
        /// Video source.
        /// </summary>
        /// 
        /// <remarks>URL, which provides MJPEG stream.</remarks>
        /// 
        public string Source
        {
            get { return _source; }
            set
            {
                _source = value;
                ReloadThread();
            }
        }

        private void ReloadThread()
        {
            if (_thread != null)
                _reloadEvent.Set();
        }

        /// <summary>
        /// Login value.
        /// </summary>
        /// 
        /// <remarks>Login required to access video source.</remarks>
        /// 
        public string Login
        {
            get { return _userName; }
            set { _userName = value; }
        }

        /// <summary>
        /// Password value.
        /// </summary>
        /// 
        /// <remarks>Password required to access video source.</remarks>
        /// 
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        /// <summary>
        /// Gets or sets proxy information for the request.
        /// </summary>
        /// 
        /// <remarks><para>The local computer or application config file may specify that a default
        /// proxy to be used. If the Proxy property is specified, then the proxy settings from the Proxy
        /// property overridea the local computer or application config file and the instance will use
        /// the proxy settings specified. If no proxy is specified in a config file
        /// and the Proxy property is unspecified, the request uses the proxy settings
        /// inherited from Internet Explorer on the local computer. If there are no proxy settings
        /// in Internet Explorer, the request is sent directly to the server.
        /// </para></remarks>
        /// 
        public IWebProxy Proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }

        /// <summary>
        /// User agent to specify in HTTP request header.
        /// </summary>
        /// 
        /// <remarks><para>Some IP cameras check what is the requesting user agent and depending
        /// on it they provide video in different formats or do not provide it at all. The property
        /// sets the value of user agent string, which is sent to camera in request header.
        /// </para>
        /// 
        /// <para>Default value is set to "Mozilla/5.0". If the value is set to <see langword="null"/>,
        /// the user agent string is not sent in request header.</para>
        /// </remarks>
        /// 
        public string HttpUserAgent
        {
            get { return _userAgent; }
            set { _userAgent = value; }
        }

        /// <summary>
        /// Received frames count.
        /// </summary>
        /// 
        /// <remarks>Number of frames the video source provided from the moment of the last
        /// access to the property.
        /// </remarks>
        /// 
        public int FramesReceived
        {
            get
            {
                int frames = _framesReceived;
                _framesReceived = 0;
                return frames;
            }
        }

        /// <summary>
        /// Received bytes count.
        /// </summary>
        /// 
        /// <remarks>Number of bytes the video source provided from the moment of the last
        /// access to the property.
        /// </remarks>
        /// 
        public long BytesReceived
        {
            get
            {
                long bytes = _bytesReceived;
                _bytesReceived = 0;
                return bytes;
            }
        }

        /// <summary>
        /// Request timeout value.
        /// </summary>
        /// 
        /// <remarks>The property sets timeout value in milliseconds for web requests.
        /// Default value is 10000 milliseconds.</remarks>
        /// 
        public int RequestTimeout
        {
            get { return _requestTimeout; }
            set { _requestTimeout = value; }
        }

        /// <summary>
        /// State of the video source.
        /// </summary>
        /// 
        /// <remarks>Current state of video source object - running or not.</remarks>
        /// 
        public bool IsRunning
        {
            get
            {
                return IsThreadRunning();
            }
        }

        private bool IsThreadRunning()
        {
            bool isRunning = false;

            if (_thread != null)
            {
                // check thread status
                if (_thread.Join(0) == false)
                {
                    isRunning = true;
                }
                else
                {
                    // the thread is not running, so free resources
                    FreeThreadResources();
                }
            }
            return isRunning;
        }

        private bool IsReloadRequested
        {
            get { return _reloadEvent.WaitOne(0, false); }
        }

        private bool IsStopRequested
        {
            get { return _stopEvent.WaitOne(0, false); }
        }

        /// <summary>
        /// Force using of basic authentication when connecting to the video source.
        /// </summary>
        /// 
        /// <remarks><para>For some IP cameras (TrendNET IP cameras, for example) using standard .NET's authentication via credentials
        /// does not seem to be working (seems like camera does not request for authentication, but expects corresponding headers to be
        /// present on connection request). So this property allows to force basic authentication by adding required HTTP headers when
        /// request is sent.</para>
        /// 
        /// <para>Default value is set to <see langword="false"/>.</para>
        /// </remarks>
        /// 
        public bool ForceBasicAuthentication
        {
            get { return _forceBasicAuthentication; }
            set { _forceBasicAuthentication = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MJPEGStream"/> class.
        /// </summary>
        /// 
        public MJPEGStream() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MJPEGStream"/> class.
        /// </summary>
        /// 
        /// <param name="source">URL, which provides MJPEG stream.</param>
        /// 
        public MJPEGStream(string source)
        {
            _source = source;
        }

        /// <summary>
        /// Start video source.
        /// </summary>
        /// 
        /// <remarks>Starts video source and return execution to caller. Video source
        /// object creates background thread and notifies about new frames with the
        /// help of <see cref="NewFrame"/> event.</remarks>
        /// 
        /// <exception cref="ArgumentException">Video source is not specified.</exception>
        /// 
        public void Start()
        {
            if (!IsRunning)
            {
                // check source
                if ((_source == null) || (_source == string.Empty))
                    throw new ArgumentException("Video source is not specified.");

                _framesReceived = 0;
                _bytesReceived = 0;

                // create events
                _stopEvent = new ManualResetEvent(false);
                _reloadEvent = new ManualResetEvent(false);

                // create and start new thread
                _thread = new Thread(new ThreadStart(WorkerThread))
                {
                    Name = _source
                };
                _thread.Start();
            }
        }

        /// <summary>
        /// Signal video source to stop its work.
        /// </summary>
        /// 
        /// <remarks>Signals video source to stop its background thread, stop to
        /// provide new frames and free resources.</remarks>
        /// 
        public void SignalToStop()
        {
            // stop thread
            if (_thread != null)
            {
                // signal to stop
                _stopEvent.Set();
            }
        }

        /// <summary>
        /// Wait for video source has stopped.
        /// </summary>
        /// 
        /// <remarks>Waits for source stopping after it was signalled to stop using
        /// <see cref="SignalToStop"/> method.</remarks>
        /// 
        public void WaitForStop()
        {
            if (_thread != null)
            {
                // wait for thread stop
                _thread.Join();

                FreeThreadResources();
            }
        }

        /// <summary>
        /// Stop video source.
        /// </summary>
        /// 
        /// <remarks><para>Stops video source aborting its thread.</para>
        /// 
        /// <para><note>Since the method aborts background thread, its usage is highly not preferred
        /// and should be done only if there are no other options. The correct way of stopping camera
        /// is <see cref="SignalToStop">signaling it stop</see> and then
        /// <see cref="WaitForStop">waiting</see> for background thread's completion.</note></para>
        /// </remarks>
        /// 
        public void Stop()
        {
            if (IsRunning)
            {
                _stopEvent.Set();
                _thread.Abort();
                WaitForStop();
            }
        }

        /// <summary>
        /// Free resource.
        /// </summary>
        /// 
        private void FreeThreadResources()
        {
            _thread = null;

            // release events
            _stopEvent.Close();
            _stopEvent = null;
            _reloadEvent.Close();
            _reloadEvent = null;
        }

        private void WorkerThread()
        {
            while (!IsStopRequested)
            {
                MJPEGStreamParser parser;
                Boundary boundary;
                WebResponse response = null;

                // reset reload event
                _reloadEvent.Reset();

                try
                {
                    response = GetResponse();

                    boundary = Boundary.FromResponse(response);
                    parser = new MJPEGStreamParser(boundary, JPEG_HEADER_BYTES);

                    using (Stream stream = GetResponseStream(response))
                    {
                        while (!IsStopRequested && !IsReloadRequested)
                        {
                            _bytesReceived += parser.Read(stream);

                            if (boundary.HasValue && !boundary.IsChecked)
                            {
                                boundary.FixMalformedBoundary(parser);
                            }

                            if (boundary.IsValid)
                            {
                                parser.DetectFrame();

                                if (parser.HasFrame)
                                {
                                    _framesReceived++;

                                    if (NewFrame != null && !IsStopRequested)
                                    {
                                        using (Bitmap frame = parser.GetFrame())
                                        {
                                            NewFrame(this, new NewFrameEventArgs(frame));
                                        }
                                        parser.RemoveFrame();
                                    }
                                }
                            }
                        }
                    }
                }
                catch (ApplicationException)
                {
                    // do nothing for Application Exception, which we raised on our own
                    // wait for a while before the next try
                    Thread.Sleep(250);
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (Exception exception)
                {
                    // provide information to clients
                    if (VideoSourceError != null)
                    {
                        VideoSourceError(this, new VideoSourceErrorEventArgs(exception.Message, exception));
                    }
                    // wait for a while before the next try
                    Thread.Sleep(250);
                }
                finally
                {
                    // close response
                    if (response != null)
                    {
                        response.Close();
                    }
                }

                if (IsStopRequested)
                    break;
            }

            if (PlayingFinished != null)
            {
                PlayingFinished(this, ReasonToFinishPlaying.StoppedByUser);
            }
        }

        private WebResponse GetResponse()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_source);

            try
            {
                SetUserAgent(request);

                SetProxy(request);
                SetRequestTimeout(request);
                SetCredentials(request);
                SetConnectionGroupName(request);

                if (_forceBasicAuthentication)
                {
                    SetBasicAuthentication(request);
                }

                return request.GetResponse();
            }
            catch (WebException)
            {
                if (request != null)
                {
                    request.Abort();
                }

                throw;
            }
        }

        private void SetUserAgent(HttpWebRequest request)
        {
            if (_userAgent != null)
            {
                request.UserAgent = _userAgent;
            }
        }

        private void SetProxy(HttpWebRequest request)
        {
            if (_proxy != null)
            {
                request.Proxy = _proxy;
            }
        }

        private void SetRequestTimeout(HttpWebRequest request)
        {
            request.Timeout = _requestTimeout;
        }

        private void SetCredentials(HttpWebRequest request)
        {
            if (!string.IsNullOrEmpty(_userName) && _password != null)
                request.Credentials = new NetworkCredential(_userName, _password);
        }

        private void SetConnectionGroupName(HttpWebRequest request)
        {
#if !NETSTANDARD2_0
            if (_useSeparateConnectionGroup)
                request.ConnectionGroupName = GetHashCode().ToString();
#endif
        }

        private void SetBasicAuthentication(HttpWebRequest request)
        {
            const string HEADER_KEY = "Authorization";

            string payload = string.Format("{0}:{1}", _userName, _password);
            byte[] payloadBytes = Encoding.Default.GetBytes(payload);

            string encodedPayload = Convert.ToBase64String(payloadBytes);

            request.Headers[HEADER_KEY] = string.Format("{0} {1}", "Basic", encodedPayload);
        }

        private Stream GetResponseStream(WebResponse response)
        {
            Stream stream = response.GetResponseStream();

            if (!stream.CanTimeout)
            {
                stream = new TimeoutStream(stream);
            }

            SetTimeout(stream);

            return stream;
        }

        private void SetTimeout(Stream stream)
        {
            if (stream.CanTimeout)
            {
                stream.ReadTimeout = _requestTimeout;
            }
        }
    }
}
