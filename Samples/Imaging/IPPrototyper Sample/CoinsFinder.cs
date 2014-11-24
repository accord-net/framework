// Sample of IPPrototyper usage
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

using AForge;
using AForge.Math.Geometry;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Imaging.IPPrototyper;

namespace IPPrototyperSample
{
    public class CoinsFinder : IImageProcessingRoutine
    {
        // Image processing routine's name
        public string Name
        {
            get { return "IPPrototyper Sample"; }
        }

        // Process specified image trying to recognize counter's image
        public void Process( Bitmap image, IImageProcessingLog log )
        {
            log.AddMessage( "Image size: " + image.Width + " x " + image.Height );

            // 1- grayscale image
            Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply( image );
            log.AddImage( "Grayscale", grayImage );

            // 2 - Otsu thresholding
            OtsuThreshold threshold = new OtsuThreshold( );
            Bitmap binaryImage = threshold.Apply( grayImage );
            log.AddImage( "Binary", binaryImage );
            log.AddMessage( "Otsu threshold: " + threshold.ThresholdValue );

            // 3 - Blob counting
            BlobCounter blobCounter = new BlobCounter( );
            blobCounter.FilterBlobs = true;
            blobCounter.MinWidth = 24;
            blobCounter.MinWidth = 24;

            blobCounter.ProcessImage( binaryImage );
            Blob[] blobs = blobCounter.GetObjectsInformation( );

            log.AddMessage( "Found blobs (min width/height = 24): " + blobs.Length );

            // 4 - check shape of each blob
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker( );

            log.AddMessage( "Found coins: " );
            int count = 0;

            // create graphics object for drawing on image
            Graphics g = Graphics.FromImage( image );
            Pen pen = new Pen( Color.Red, 3 );

            foreach ( Blob blob in blobs )
            {
                List<IntPoint> edgePoint = blobCounter.GetBlobsEdgePoints( blob );

                // check if shape looks like a circle
                AForge.Point center;
                float radius;

                if ( shapeChecker.IsCircle( edgePoint, out center, out radius ) )
                {
                    count++;

                    log.AddMessage( string.Format( "  {0}: center = ({1}, {2}), radius = {3}",
                        count, center.X, center.Y, radius ) );

                    // highlight coin
                    g.DrawEllipse( pen, (int) ( center.X - radius ), (int) ( center.Y - radius ),
                        (int) ( radius * 2 ), (int) ( radius * 2 ) );
                }
            }

            g.Dispose( );
            pen.Dispose( );

            log.AddMessage( "Total coins: " + count);
            log.AddImage( "Coins", image );
        }
    }
}
