// Accord Vision Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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

namespace Accord.Vision.Tracking
{
    using AForge.Imaging;
    using AForge.Imaging.Filters;
    using Accord.Imaging;
    using System;
    using System.Drawing;
    using AForge;
    using Accord.Imaging.Moments;

    /// <summary>
    ///   Blob object tracker.
    /// </summary>
    /// 
    public class HslBlobTracker : IObjectTracker
    {
        private TrackingObject trackingObject;
        private HSLFiltering filter;
        private UnmanagedImage filterImage;
        private BlobCounter blobCounter;
        private bool extract;
        private bool rotation;


        /// <summary>
        /// Gets or sets the maximum width of tracked objects.
        /// </summary>
        /// 
        public int MaxWidth
        {
            get { return blobCounter.MaxWidth; }
            set { blobCounter.MaxWidth = value; }
        }

        /// <summary>
        /// Gets or sets the maximum height of tracked objects.
        /// </summary>
        /// 
        public int MaxHeight
        {
            get { return blobCounter.MaxHeight; }
            set { blobCounter.MaxHeight = value; }
        }

        /// <summary>
        /// Gets or sets the minimum width of tracked objects.
        /// </summary>
        /// 
        public int MinWidth
        {
            get { return blobCounter.MinWidth; }
            set { blobCounter.MinWidth = value; }
        }

        /// <summary>
        /// Gets or sets the minimum height of tracked objects.
        /// </summary>
        /// 
        public int MinHeight
        {
            get { return blobCounter.MinHeight; }
            set { blobCounter.MinHeight = value; }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether the tracker
        ///   should extract the object image from the source. The
        ///   extracted image will be available in <see cref="Accord.Vision.Tracking.TrackingObject.Image"/>.
        /// </summary>
        /// 
        public bool Extract
        {
            get { return extract; }
            set { extract = value; }
        }

        /// <summary>
        ///   Gets or sets whether the tracker should compute blob's orientation.
        /// </summary>
        /// 
        public bool ComputeOrientation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        /// <summary>
        ///   Gets the HSL filter used in color segmentation.
        /// </summary>
        /// 
        /// <value>The HSL filter used in segmentation.</value>
        /// 
        public HSLFiltering Filter
        {
            get { return filter; }
        }

        /// <summary>
        ///   Gets the HSL filtered image.
        /// </summary>
        /// 
        public UnmanagedImage FilterImage
        {
            get { return filterImage; }
        }

        /// <summary>
        /// Gets the current location of the object being tracked.
        /// </summary>
        /// 
        public TrackingObject TrackingObject
        {
            get { return trackingObject; }
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="HslBlobTracker"/> class.
        /// </summary>
        /// 
        public HslBlobTracker()
        {
            this.filter = new HSLFiltering();
            this.blobCounter = new BlobCounter();
            this.trackingObject = new TrackingObject();

            blobCounter.CoupledSizeFiltering = false;
            blobCounter.FilterBlobs = true;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="HslBlobTracker"/> class.
        /// </summary>
        /// 
        /// <param name="filter">The filter.</param>
        /// 
        public HslBlobTracker(HSLFiltering filter)
        {
            this.filter = filter;
            this.blobCounter = new BlobCounter();
            this.trackingObject = new TrackingObject();

            blobCounter.CoupledSizeFiltering = false;
            blobCounter.FilterBlobs = true;
        }



        /// <summary>
        /// Process a new video frame.
        /// </summary>
        /// 
        public void ProcessFrame(UnmanagedImage frame)
        {
            filterImage = filter.Apply(frame);

            Blob blob = extractBlob();

            if (blob == null)
            {
                trackingObject.Reset();
                return;
            }


            trackingObject.Rectangle = blob.Rectangle;
            trackingObject.Center = (IntPoint)blob.CenterOfGravity;

            if (rotation)
            {
                // Locate moments
                CentralMoments moments = new CentralMoments();
                moments.Compute(filterImage, blob.Rectangle);
                trackingObject.Angle = moments.GetOrientation();
            }


            if (extract)
            {
                blobCounter.ExtractBlobsImage(filterImage, blob, false);
                trackingObject.Image = blob.Image;
            }
        }

        private Blob extractBlob()
        {
            blobCounter.ProcessImage(filterImage);

            // get information about blobs
            Blob[] blobs = blobCounter.GetObjectsInformation();

            // find the biggest blob
            int maxSize = 0;
            Blob biggestBlob = null;

            for (int i = 0; i < blobs.Length; i++)
            {
                int size = blobs[i].Rectangle.Width * blobs[i].Rectangle.Height;

                if (size > maxSize)
                {
                    maxSize = size;
                    biggestBlob = blobs[i];
                }
            }
            return biggestBlob;
        }


        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            trackingObject.Reset();
        }
    }
}
