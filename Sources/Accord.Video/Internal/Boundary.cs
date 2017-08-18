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
    using System.Text;
    using System.Net;

    internal class Boundary
    {
        private readonly static Encoding _encoding = Encoding.ASCII;

        private readonly StringBuilder _builder;

        private bool _isChecked = false;

        public Boundary()
        {
            _builder = new StringBuilder();
        }

        public Boundary(string boundary)
        {
            _builder = new StringBuilder(boundary);
        }

        public Boundary(byte[] boundary)
        {
            string content = _encoding.GetString(boundary);
            _builder = new StringBuilder(content);
        }

        public string Content
        {
            get { return _builder.ToString(); }
        }

        public int Length
        {
            get { return _builder.Length; }
        }

        public bool HasValue
        {
            get { return Length != 0; }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; }
        }

        public void Prepend(char c)
        {
            _builder.Insert(0, c);
        }

        /// <summary>
        /// Some IP cameras, like AirLink, claim that boundary is "myboundary",
        /// when it is really "--myboundary". This corrects the issue.
        /// </summary>
        /// <param name="buffer">Image data buffer</param>
        /// <param name="boundary">Existing boundary</param>
        /// <param name="position">Current position in buffer</param>
        /// <returns></returns>
        public void FixMalformedBoundary(MJPEGStreamParser imageBuffer)
        {
            byte[] buffer = (byte[])imageBuffer;

            int boundaryIndex = imageBuffer.FindImageBoundary();

            if (boundaryIndex != -1)
            {
                for (int i = boundaryIndex - 1; i >= 0; i--)
                {
                    char ch = (char)buffer[i];

                    if (ch == '\n' || ch == '\r')
                    {
                        break;
                    }

                    Prepend(ch);
                }

                IsChecked = true;
            }
        }

        public static Boundary FromResponse(WebResponse response)
        {
            string contentType = response.ContentType;

            Boundary boundary = new Boundary();

            if (IsMultipartContent(contentType))
            {
                int boundaryIndex = GetBoundaryIndex(contentType);

                if (boundaryIndex != -1)
                {
                    boundary = TrimBoundary(contentType, boundaryIndex);
                    boundary.IsChecked = false;
                }
            }
            else if (!IsOctetStream(contentType))
            {
                throw new Exception("Invalid content type.");
            }

            return boundary;
        }

        private static int GetBoundaryIndex(string contentType)
        {
            int boundaryIndex = contentType.IndexOf("boundary", 0);
            if (boundaryIndex != -1)
            {
                boundaryIndex = contentType.IndexOf("=", boundaryIndex + 8);
            }

            return boundaryIndex;
        }

        private static Boundary TrimBoundary(string contentType, int boundaryIndex)
        {
            string boundary = contentType.Substring(boundaryIndex + 1);
            string trimmedBoundary = boundary.Trim(' ', '"');

            return new Boundary(trimmedBoundary);
        }

        private static bool IsMultipartContent(string contentType)
        {
            return contentType.StartsWith("multipart") && contentType.Contains("mixed");
        }

        private static bool IsOctetStream(string contentType)
        {
            return contentType.StartsWith("application/octet-stream");
        }

        public static explicit operator string(Boundary boundary)
        {
            string content = null;

            if (boundary != null)
            {
                content = boundary.Content;
            }

            return content;
        }

        public static explicit operator byte[] (Boundary boundary)
        {
            byte[] content = null;

            if (boundary != null)
            {
                content = _encoding.GetBytes(boundary.Content);
            }

            return content;
        }
    }
}
