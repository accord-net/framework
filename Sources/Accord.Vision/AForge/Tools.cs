// AForge Vision Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace AForge.Vision
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge.Imaging;
    using AForge.Imaging.Filters;

    internal static class Tools
    {
        // Get grayscale image out of the specified one
        public static void ConvertToGrayscale( UnmanagedImage source, UnmanagedImage destination )
        {
            if ( source.PixelFormat != PixelFormat.Format8bppIndexed )
            {
                Grayscale.CommonAlgorithms.BT709.Apply( source, destination );
            }
            else
            {
                source.Copy( destination );
            }
        }
    }
}
