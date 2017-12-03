// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace Accord.Imaging
{
    using System;
    using System.IO;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge;
    using System.Net;
    using Accord.Imaging.Formats;

    /// <summary>
    /// Core image relatad methods.
    /// </summary>
    /// 
    /// <remarks>All methods of this class are static and represent general routines
    /// used by different image processing classes.</remarks>
    /// 
    public static class Image
    {
        /// <summary>
        /// Check if specified 8 bpp image is grayscale.
        /// </summary>
        /// 
        /// <param name="image">Image to check.</param>
        /// 
        /// <returns>Returns <b>true</b> if the image is grayscale or <b>false</b> otherwise.</returns>
        /// 
        /// <remarks>The methods checks if the image is a grayscale image of 256 gradients.
        /// The method first examines if the image's pixel format is
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format8bppIndexed</see>
        /// and then it examines its palette to check if the image is grayscale or not.</remarks>
        /// 
        /// <seealso cref="IsColor8bpp(Bitmap)"/>
        /// 
        public static bool IsGrayscale(this Bitmap image)
        {
            // check pixel format
            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                // check palette
                ColorPalette cp = image.Palette;

                // init palette
                for (int i = 0; i < 256; i++)
                {
                    Color c = cp.Entries[i];
                    if ((c.R != i) || (c.G != i) || (c.B != i))
                        return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if specified 8 bpp image is contains color-indexed pixels instead of intensity values.
        /// </summary>
        /// 
        /// <param name="image">Image to check.</param>
        /// 
        /// <returns>Returns <b>true</b> if the image is color-indexed or <b>false</b> otherwise.</returns>
        /// 
        /// <seealso cref="IsGrayscale(Bitmap)"/>
        /// 
        public static bool IsColor8bpp(this Bitmap image)
        {
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
                return false;

            return !image.IsGrayscale();
        }

        /// <summary>
        /// Create and initialize new 8 bpp grayscale image.
        /// </summary>
        /// 
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// 
        /// <returns>Returns the created grayscale image.</returns>
        /// 
        /// <remarks>The method creates new 8 bpp grayscale image and initializes its palette.
        /// Grayscale image is represented as
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format8bppIndexed</see>
        /// image with palette initialized to 256 gradients of gray color.</remarks>
        /// 
        public static Bitmap CreateGrayscaleImage(int width, int height)
        {
            // create new image
            Bitmap image = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

            // set palette to grayscale
            SetGrayscalePalette(image);

            // return new image
            return image;
        }

        /// <summary>
        /// Set pallete of the 8 bpp indexed image to grayscale.
        /// </summary>
        /// 
        /// <param name="image">Image to initialize.</param>
        /// 
        /// <remarks>The method initializes palette of
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format8bppIndexed</see>
        /// image with 256 gradients of gray color.</remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Provided image is not 8 bpp indexed image.</exception>
        /// 
        public static void SetGrayscalePalette(this Bitmap image)
        {
            // check pixel format
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new UnsupportedImageFormatException("Source image is not 8 bpp image.");

            // get palette
            ColorPalette cp = image.Palette;

            // init palette
            for (int i = 0; i < 256; i++)
                cp.Entries[i] = Color.FromArgb(i, i, i);

            // set palette back
            image.Palette = cp;
        }

        /// <summary>
        /// Clone image.
        /// </summary>
        /// 
        /// <param name="source">Source image.</param>
        /// <param name="format">Pixel format of result image.</param>
        /// 
        /// <returns>Returns clone of the source image with specified pixel format.</returns>
        ///
        /// <remarks>The original <see cref="System.Drawing.Bitmap.Clone(System.Drawing.Rectangle, System.Drawing.Imaging.PixelFormat)">Bitmap.Clone()</see>
        /// does not produce the desired result - it does not create a clone with specified pixel format.
        /// More of it, the original method does not create an actual clone - it does not create a copy
        /// of the image. That is why this method was implemented to provide the functionality.</remarks> 
        ///
        public static Bitmap Clone(this Bitmap source, PixelFormat format)
        {
            // copy image if pixel format is the same
            if (source.PixelFormat == format)
                return Clone(source);

            int width = source.Width;
            int height = source.Height;

            // create new image with desired pixel format
            Bitmap destination = new Bitmap(width, height, format);

            return Copy(source, destination);
        }

        /// <summary>
        /// Copy image.
        /// </summary>
        /// 
        /// <param name="source">Source image.</param>
        /// <param name="destination">Destination image. If set to null, a new image will be created.</param>
        /// 
        /// <returns>Returns clone of the source image with specified pixel format.</returns>
        ///
        public static Bitmap Copy(this Bitmap source, Bitmap destination)
        {
            int width = source.Width;
            int height = source.Height;

            if (destination == null)
                destination = new Bitmap(width, height, source.PixelFormat);

            // draw source image on the new one using Graphics
            using (Graphics g = Graphics.FromImage(destination))
                g.DrawImage(source, 0, 0, width, height);

            return destination;
        }

        /// <summary>
        /// Clone image.
        /// </summary>
        /// 
        /// <param name="bytes">Source image as an array of bytes.</param>
        /// 
        /// <returns>Returns clone of the source image with specified pixel format.</returns>
        ///
        /// <remarks>The original <see cref="System.Drawing.Bitmap.Clone(System.Drawing.Rectangle, System.Drawing.Imaging.PixelFormat)">Bitmap.Clone()</see>
        /// does not produce the desired result - it does not create a clone with specified pixel format.
        /// More of it, the original method does not create an actual clone - it does not create a copy
        /// of the image. That is why this method was implemented to provide the functionality.</remarks> 
        ///
        public static Bitmap Clone(this byte[] bytes)
        {
            return (Bitmap)Bitmap.FromStream(new MemoryStream(bytes));
        }

        /// <summary>
        /// Clone image.
        /// </summary>
        /// 
        /// <param name="source">Source image.</param>
        /// 
        /// <returns>Return clone of the source image.</returns>
        /// 
        /// <remarks>The original <see cref="System.Drawing.Bitmap.Clone(System.Drawing.Rectangle, System.Drawing.Imaging.PixelFormat)">Bitmap.Clone()</see>
        /// does not produce the desired result - it does not create an actual clone (it does not create a copy
        /// of the image). That is why this method was implemented to provide the functionality.</remarks> 
        /// 
        public static Bitmap Clone(this Bitmap source)
        {
            // lock source bitmap data
            BitmapData sourceData = source.LockBits(ImageLockMode.ReadOnly);
            Bitmap destination;

            try
            {
                // create new image
                destination = Clone(sourceData);
            }
            finally
            {
                // unlock source image
                source.UnlockBits(sourceData);
            }

            if (
                (source.PixelFormat == PixelFormat.Format1bppIndexed) ||
                (source.PixelFormat == PixelFormat.Format4bppIndexed) ||
                (source.PixelFormat == PixelFormat.Format8bppIndexed) ||
                (source.PixelFormat == PixelFormat.Indexed))
            {
                ColorPalette srcPalette = source.Palette;
                ColorPalette dstPalette = destination.Palette;

                // copy pallete
                for (int i = 0; i < dstPalette.Entries.Length; i++)
                    dstPalette.Entries[i] = srcPalette.Entries[i];

                destination.Palette = dstPalette;
            }

            return destination;
        }

        /// <summary>
        ///   Converts a 8-bpp color image into a 8-bpp grayscale image, setting its color 
        ///   palette to grayscale and replacing palette indices with their grayscale values.
        /// </summary>
        /// 
        /// <param name="bitmap">The bitmap to be converted.</param>
        /// 
        public static void ConvertColor8bppToGrayscale8bpp(this Bitmap bitmap)
        {
            if (bitmap.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new UnsupportedImageFormatException("Only 8-bpp images are supported.");

            // lock source bitmap data
            BitmapData sourceData = bitmap.LockBits(ImageLockMode.ReadWrite);
            ColorPalette palette = bitmap.Palette;

            int height = bitmap.Height;
            int width = bitmap.Width;
            int stride = sourceData.Stride;
            int offset = stride - width;

            try
            {
                unsafe
                {
                    // base pointers
                    byte* src = (byte*)sourceData.Scan0.ToPointer();

                    for (int y = 0; y < height; y++)
                    { 
                        for (int x = 0; x < width; x++, src++)
                        {
                            Color entry = palette.Entries[*src];
#if DEBUG
                            if (entry.R != entry.G || entry.R != entry.B)
                                throw new Exception();
#endif
                            *src = entry.R;
                        }

                        src += offset;
                    }
                }
            }
            finally
            {
                // unlock source image
                bitmap.UnlockBits(sourceData);
            }

            SetGrayscalePalette(bitmap);
        }

        /// <summary>
        /// Clone image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        ///
        /// <returns>Clones image from source image data. The message does not clone pallete in the
        /// case if the source image has indexed pixel format.</returns>
        /// 
        public static Bitmap Clone(this BitmapData sourceData)
        {
            return Copy(sourceData, null);
        }

        private static Bitmap Copy(this BitmapData sourceData, Bitmap destination)
        {
            if (destination == null)
            {
                // create new image
                destination = new Bitmap(sourceData.Width, sourceData.Height, sourceData.PixelFormat);
            }

            // get source image size
            int width = sourceData.Width;
            int height = sourceData.Height;

            // lock destination bitmap data
            BitmapData destinationData = destination.LockBits(ImageLockMode.WriteOnly);

            System.Diagnostics.Debug.Assert(destinationData.Stride == sourceData.Stride);

            SystemTools.CopyUnmanagedMemory(destinationData.Scan0, sourceData.Scan0, height * sourceData.Stride);

            // unlock destination image
            destination.UnlockBits(destinationData);

            return destination;
        }

        /// <summary>
        /// Format an image.
        /// </summary>
        /// 
        /// <param name="image">Source image to format.</param>
        /// 
        /// <remarks><para>Formats the image to one of the formats, which are supported
        /// by the <b>AForge.Imaging</b> library. The image is left untouched in the
        /// case if it is already of
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format24bppRgb</see> or
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format32bppRgb</see> or
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format32bppArgb</see> or
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format48bppRgb</see> or
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format64bppArgb</see>
        /// format or it is <see cref="IsGrayscale">grayscale</see>, otherwise the image
        /// is converted to <see cref="System.Drawing.Imaging.PixelFormat">Format24bppRgb</see>
        /// format.</para>
        /// 
        /// <para><note>The method is deprecated and <see cref="Clone(Bitmap, PixelFormat)"/> method should
        /// be used instead with specifying desired pixel format.</note></para>
        /// </remarks>
        ///
        [Obsolete("Use Clone(Bitmap, PixelFormat) method instead and specify desired pixel format")]
        public static void FormatImage(ref Bitmap image)
        {
            if (
                (image.PixelFormat != PixelFormat.Format24bppRgb) &&
                (image.PixelFormat != PixelFormat.Format32bppRgb) &&
                (image.PixelFormat != PixelFormat.Format32bppArgb) &&
                (image.PixelFormat != PixelFormat.Format48bppRgb) &&
                (image.PixelFormat != PixelFormat.Format64bppArgb) &&
                (image.PixelFormat != PixelFormat.Format16bppGrayScale) &&
                (IsGrayscale(image) == false)
                )
            {
                Bitmap tmp = image;

                // convert to 24 bits per pixel
                image = Clone(tmp, PixelFormat.Format24bppRgb);

                // delete old image
                tmp.Dispose();
            }
        }

        /// <summary>
        /// Load bitmap from file.
        /// </summary>
        /// 
        /// <param name="fileName">File name to load bitmap from.</param>
        /// 
        /// <returns>Returns loaded bitmap.</returns>
        /// 
        /// <remarks><para>The method is provided as an alternative of <see cref="System.Drawing.Image.FromFile(string)"/>
        /// method to solve the issues of locked file. The standard .NET's method locks the source file until
        /// image's object is disposed, so the file can not be deleted or overwritten. This method workarounds the issue and
        /// does not lock the source file.</para>
        /// </remarks>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Imaging\Formats\PNMCodecTest.cs" region="doc_load" />
        /// </example>
        /// 
        public static System.Drawing.Bitmap FromFile(string fileName)
        {
            return ImageDecoder.DecodeFromFile(fileName);
        }

        /// <summary>
        /// Converts a bitmap with 16 bits per plane to a bitmap with 8 bits per plane.
        /// </summary>
        /// 
        /// <param name="bitmap">Source image to convert.</param>
        /// 
        /// <returns>Returns new image which is a copy of the source image but with 8 bits per plane.</returns>
        /// 
        /// <remarks><para>The routine does the next pixel format conversions:
        /// <list type="bullet">
        /// <item><see cref="PixelFormat.Format16bppGrayScale">Format16bppGrayScale</see> to
        /// <see cref="PixelFormat.Format8bppIndexed">Format8bppIndexed</see> with grayscale palette;</item>
        /// <item><see cref="PixelFormat.Format48bppRgb">Format48bppRgb</see> to
        /// <see cref="PixelFormat.Format24bppRgb">Format24bppRgb</see>;</item>
        /// <item><see cref="PixelFormat.Format64bppArgb">Format64bppArgb</see> to
        /// <see cref="PixelFormat.Format32bppArgb">Format32bppArgb</see>;</item>
        /// <item><see cref="PixelFormat.Format64bppPArgb">Format64bppPArgb</see> to
        /// <see cref="PixelFormat.Format32bppPArgb">Format32bppPArgb</see>.</item>
        /// </list>
        /// </para></remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Invalid pixel format of the source image.</exception>
        /// 
        public static Bitmap Convert16bppTo8bpp(this Bitmap bitmap)
        {
            Bitmap newImage = null;

            // get image size
            int width = bitmap.Width;
            int height = bitmap.Height;

            // create new image depending on source image format
            switch (bitmap.PixelFormat)
            {
                case PixelFormat.Format16bppGrayScale:
                    // create new grayscale image
                    newImage = CreateGrayscaleImage(width, height);
                    break;

                case PixelFormat.Format48bppRgb:
                    // create new color 24 bpp image
                    newImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                    break;

                case PixelFormat.Format64bppArgb:
                    // create new color 32 bpp image
                    newImage = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                    break;

                case PixelFormat.Format64bppPArgb:
                    // create new color 32 bpp image
                    newImage = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
                    break;

                default:
                    throw new UnsupportedImageFormatException("Invalid pixel format of the source image.");
            }

            return Convert16bppTo8bpp(bitmap, newImage);
        }

        /// <summary>
        /// Converts a bitmap with 16 bits per plane to a bitmap with 8 bits per plane.
        /// </summary>
        /// 
        /// <param name="source">Source image to convert.</param>
        /// <param name="destination">Destination image to store the conversion. If set to null, a new image will be created.</param>
        /// 
        /// <returns>Returns new image which is a copy of the source image but with 8 bits per plane.</returns>
        /// 
        /// <remarks><para>The routine does the next pixel format conversions:
        /// <list type="bullet">
        /// <item><see cref="PixelFormat.Format16bppGrayScale">Format16bppGrayScale</see> to
        /// <see cref="PixelFormat.Format8bppIndexed">Format8bppIndexed</see> with grayscale palette;</item>
        /// <item><see cref="PixelFormat.Format48bppRgb">Format48bppRgb</see> to
        /// <see cref="PixelFormat.Format24bppRgb">Format24bppRgb</see>;</item>
        /// <item><see cref="PixelFormat.Format64bppArgb">Format64bppArgb</see> to
        /// <see cref="PixelFormat.Format32bppArgb">Format32bppArgb</see>;</item>
        /// <item><see cref="PixelFormat.Format64bppPArgb">Format64bppPArgb</see> to
        /// <see cref="PixelFormat.Format32bppPArgb">Format32bppPArgb</see>.</item>
        /// </list>
        /// </para></remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Invalid pixel format of the source image.</exception>
        /// 
        public static Bitmap Convert16bppTo8bpp(this Bitmap source, Bitmap destination)
        {
            // get image size
            int width = source.Width;
            int height = source.Height;
            int layers = GetPixelFormatSizeInBytes(destination);

            // lock both images
            BitmapData sourceData = source.LockBits(ImageLockMode.ReadOnly);
            BitmapData newData = destination.LockBits(ImageLockMode.ReadWrite);

            unsafe
            {
                // base pointers
                byte* src = (byte*)sourceData.Scan0.ToPointer();
                byte* dst = (byte*)newData.Scan0.ToPointer();

                // image strides
                int sourceStride = sourceData.Stride;
                int newStride = newData.Stride;

                for (int y = 0; y < height; y++)
                {
                    ushort* s = (ushort*)(src + y * sourceStride);
                    byte* d = (byte*)(dst + y * newStride);

                    for (int x = 0, lineSize = width * layers; x < lineSize; x++, s++, d++)
                        *d = (byte)(*s >> 8);
                }
            }

            // unlock both image
            source.UnlockBits(sourceData);
            destination.UnlockBits(newData);

            return destination;
        }

        /// <summary>
        /// Load bitmap from URL.
        /// </summary>
        /// 
        /// <param name="url">URL to load bitmap from.</param>
        /// 
        /// <returns>Returns loaded bitmap.</returns>
        /// 
        public static Bitmap FromUrl(string url)
        {
            return FromUrl(url, String.Empty);
        }

        /// <summary>
        /// Load bitmap from URL.
        /// </summary>
        /// 
        /// <param name="url">URL to load bitmap from.</param>
        /// <param name="localPath">The local directory where the file should be stored.</param>
        /// 
        /// <returns>Returns loaded bitmap.</returns>
        /// 
        public static Bitmap FromUrl(string url, string localPath)
        {
            string name = System.IO.Path.GetFileName(url);
            string downloadedFileName = System.IO.Path.Combine(localPath, name);

            if (!File.Exists(downloadedFileName))
            {
#if NET35
                if (localPath == null || String.IsNullOrEmpty(localPath.Trim()))
#else
                if (!String.IsNullOrWhiteSpace(localPath))
#endif
                    Directory.CreateDirectory(localPath);

                Console.WriteLine("Downloading {0}", url);
                using (var client = ExtensionMethods.NewWebClient())
                    client.DownloadFileWithRetry(url, downloadedFileName);
            }

            return FromFile(downloadedFileName);
        }

        /// <summary>
        /// Convert bitmap with 8 bits per plane to a bitmap with 16 bits per plane.
        /// </summary>
        /// 
        /// <param name="bitmap">Source image to convert.</param>
        /// 
        /// <returns>Returns new image which is a copy of the source image but with 16 bits per plane.</returns>
        /// 
        /// <remarks><para>The routine does the next pixel format conversions:
        /// <list type="bullet">
        /// <item><see cref="PixelFormat.Format8bppIndexed">Format8bppIndexed</see> (grayscale palette assumed) to
        /// <see cref="PixelFormat.Format16bppGrayScale">Format16bppGrayScale</see>;</item>
        /// <item><see cref="PixelFormat.Format24bppRgb">Format24bppRgb</see> to
        /// <see cref="PixelFormat.Format48bppRgb">Format48bppRgb</see>;</item>
        /// <item><see cref="PixelFormat.Format32bppArgb">Format32bppArgb</see> to
        /// <see cref="PixelFormat.Format64bppArgb">Format64bppArgb</see>;</item>
        /// <item><see cref="PixelFormat.Format32bppPArgb">Format32bppPArgb</see> to
        /// <see cref="PixelFormat.Format64bppPArgb">Format64bppPArgb</see>.</item>
        /// </list>
        /// </para></remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Invalid pixel format of the source image.</exception>
        /// 
        public static Bitmap Convert8bppTo16bpp(this Bitmap bitmap)
        {
            Bitmap newImage = null;
            int layers = 0;

            // get image size
            int width = bitmap.Width;
            int height = bitmap.Height;

            // create new image depending on source image format
            switch (bitmap.PixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    // create new grayscale image
                    newImage = new Bitmap(width, height, PixelFormat.Format16bppGrayScale);
                    layers = 1;
                    break;

                case PixelFormat.Format24bppRgb:
                    // create new color 48 bpp image
                    newImage = new Bitmap(width, height, PixelFormat.Format48bppRgb);
                    layers = 3;
                    break;

                case PixelFormat.Format32bppArgb:
                    // create new color 64 bpp image
                    newImage = new Bitmap(width, height, PixelFormat.Format64bppArgb);
                    layers = 4;
                    break;

                case PixelFormat.Format32bppPArgb:
                    // create new color 64 bpp image
                    newImage = new Bitmap(width, height, PixelFormat.Format64bppPArgb);
                    layers = 4;
                    break;

                default:
                    throw new UnsupportedImageFormatException("Invalid pixel format of the source image.");
            }

            // lock both images
            BitmapData sourceData = bitmap.LockBits(ImageLockMode.ReadOnly);
            BitmapData newData = newImage.LockBits(ImageLockMode.ReadWrite);

            unsafe
            {
                // base pointers
                byte* src = (byte*)sourceData.Scan0.ToPointer();
                byte* dst = (byte*)newData.Scan0.ToPointer();

                // image strides
                int sourceStride = sourceData.Stride;
                int newStride = newData.Stride;

                for (int y = 0; y < height; y++)
                {
                    byte* s = (byte*)(src + y * sourceStride);
                    ushort* d = (ushort*)(dst + y * newStride);

                    for (int x = 0, lineSize = width * layers; x < lineSize; x++, s++, d++)
                        *d = (ushort)(*s << 8);
                }
            }

            // unlock both image
            bitmap.UnlockBits(sourceData);
            newImage.UnlockBits(newData);

            return newImage;
        }

        /// <summary>
        /// Gets the color depth used in a pixel format, in number of bytes per pixel.
        /// </summary>
        /// <param name="format">The pixel format.</param>
        public static int GetPixelFormatSizeInBytes(this PixelFormat format)
        {
            return System.Drawing.Image.GetPixelFormatSize(format) / 8;
        }

        /// <summary>
        /// Gets the color depth used in a pixel format, in number of bits per pixel.
        /// </summary>
        /// <param name="format">The pixel format.</param>
        public static int GetPixelFormatSize(this PixelFormat format)
        {
            return System.Drawing.Image.GetPixelFormatSize(format);
        }

        /// <summary>
        /// Gets the color depth used in an image, in number of bytes per pixel.
        /// </summary>
        /// <param name="image">The image.</param>
        public static int GetPixelFormatSizeInBytes(this Bitmap image)
        {
            return image.PixelFormat.GetPixelFormatSizeInBytes();
        }

        /// <summary>
        /// Gets the color depth used in an image, in number of bits per pixel.
        /// </summary>
        /// <param name="image">The image.</param>
        public static int GetPixelFormatSize(this Bitmap image)
        {
            return image.PixelFormat.GetPixelFormatSize();
        }

        /// <summary>
        /// Gets the color depth used in an image, in number of bytes per pixel.
        /// </summary>
        /// <param name="image">The image.</param>
        public static int GetPixelFormatSizeInBytes(this UnmanagedImage image)
        {
            return image.PixelFormat.GetPixelFormatSizeInBytes();
        }

        /// <summary>
        /// Gets the color depth used in an image, in number of bits per pixel.
        /// </summary>
        /// <param name="image">The image.</param>
        public static int GetPixelFormatSize(this UnmanagedImage image)
        {
            return image.PixelFormat.GetPixelFormatSize();
        }

    }
}
