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
    using System.Drawing;
    using AForge.Imaging;
    using AForge.Imaging.Filters;
    using AForge;

    /// <summary>
    ///   Template matching object tracker.
    /// </summary>
    /// 
    /// <remarks>
    ///   The matching tracker will track the object presented in the search window
    ///   of the first frame given to the tracker. To reset the tracker and start
    ///   tracking another object, one can call the Reset method, then set the search
    ///   window around a new object of interest present the image containing the new
    ///   object to the tracker.
    /// </remarks>
    /// 
    public class MatchingTracker : IObjectTracker
    {

        private ExhaustiveTemplateMatching matcher;
        private Rectangle searchWindow;
        private UnmanagedImage template;
        private Crop crop;
        private TrackingObject trackingObject;
        private double threshold = 0.95;

        private int steady;


        /// <summary>
        ///   Gets or sets the current search window.
        /// </summary>
        /// 
        public Rectangle SearchWindow
        {
            get { return searchWindow; }
            set { searchWindow = value; }
        }

        /// <summary>
        ///   Gets the current location of the object being tracked.
        /// </summary>
        /// 
        public TrackingObject TrackingObject
        {
            get { return trackingObject; }
        }

        /// <summary>
        ///   Gets or sets the similarity threshold to 
        ///   determine when the object has been lost.
        /// </summary>
        /// 
        public double Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether the tracker should
        ///   extract the object image from the source. The extracted image
        ///   should be stored in <see cref="TrackingObject"/>.
        /// </summary>
        /// 
        public bool Extract { get; set; }

        /// <summary>
        ///   Constructs a new <see cref="MatchingTracker"/> object tracker.
        /// </summary>
        /// 
        public MatchingTracker()
        {
            matcher = new ExhaustiveTemplateMatching(0);
            crop = new Crop(Rectangle.Empty);
            trackingObject = new TrackingObject();
        }


        /// <summary>
        ///   Process a new video frame.
        /// </summary>
        /// 
        public void ProcessFrame(UnmanagedImage frame)
        {
            if (SearchWindow.IsEmpty)
            {
                Reset(); return;
            }

            // Check if the tracker has been initialized
            if (this.template == null)
            {
                // This frame contains the template
                // and object we are trying to track
                registerTemplate(frame, SearchWindow);

                // update object position
                updateObject(SearchWindow);
            }
            else
            {
                track(frame);
            }
        }

        private void track(UnmanagedImage frame)
        {
            if (searchWindow.Width < template.Width)
                searchWindow.Width = template.Width;
            if (searchWindow.Height < template.Height)
                searchWindow.Height = template.Height;

            searchWindow.Intersect(new Rectangle(0, 0, frame.Width, frame.Height));

            if (searchWindow.Width < template.Width || 
                searchWindow.Height < template.Height)
            {
                searchWindow.Inflate((int)(0.2f * searchWindow.Width),
                                     (int)(0.2f * searchWindow.Height));
                return;
            }

            // compare two images
            TemplateMatch obj = match(frame);

            // get object information
            int width = obj.Rectangle.Width;
            int height = obj.Rectangle.Height;

            // update object position
            updateObject(obj.Rectangle);


            // check similarity level
            if (!checkSteadiness(obj))
            {
                Reset(); return;
            }

             if (obj.Similarity >= 0.99)
             {
                 registerTemplate(frame, obj.Rectangle);
             }


            // Compute a new window size
            searchWindow = obj.Rectangle;

            if (obj.Similarity < 0.98)
                searchWindow.Inflate((int)(0.2f * width), (int)(0.2f * height));

            else
                searchWindow.Inflate((int)(0.1f * width), (int)(0.1f * height));
        }

        private bool checkSteadiness(TemplateMatch match)
        {
            if (match.Similarity < threshold)
            {
                if (--steady < -10) steady = -10;
            }
            else
            {
                if (++steady > 0) steady = 0;
            }

            return (steady != -10);
        }

        private TemplateMatch match(UnmanagedImage frame)
        {
            TemplateMatch[] matchings = matcher.ProcessImage(frame, template, searchWindow);

            // Select highest match
            TemplateMatch match = matchings[0];
            double max = match.Similarity;

            for (int i = 1; i < matchings.Length; i++)
            {
                if (matchings[i].Similarity > max)
                {
                    match = matchings[i];
                    max = match.Similarity;
                }
            }

            return match;
        }

        private void registerTemplate(UnmanagedImage frame, Rectangle window)
        {
            crop.Rectangle = window;
            template = crop.Apply(frame);
        }

        private void updateObject(Rectangle window)
        {
            trackingObject.Image = template;
            trackingObject.Rectangle = window;
            trackingObject.Center = new IntPoint(
                (int)(window.X + window.Width / 2f),
                (int)(window.Y + window.Height / 2f));
        }


        /// <summary>
        ///   Resets this instance.
        /// </summary>
        /// 
        public void Reset()
        {
            this.template = null;
            this.trackingObject.Reset();
            this.steady = 0;
        }
    }
}
