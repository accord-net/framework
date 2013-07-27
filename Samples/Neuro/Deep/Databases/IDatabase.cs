// Accord.NET Sample Applications
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2012
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

namespace DeepLearning.Databases
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Media.Imaging;

    /// <summary>
    ///   Common interface for databases. You can plug your own
    ///   database in the application by implementing this interface.
    /// </summary>
    /// 
    public interface IDatabase : INotifyPropertyChanged
    {
        /// <summary>
        ///   Gets the number of classes in
        ///   the classification problem.
        /// </summary>
        /// 
        int Classes { get; }

        /// <summary>
        ///   Gets the collection of training instances.
        /// </summary>
        /// 
        ObservableCollection<Sample> Training { get; }

        /// <summary>
        ///   Gets the colleection of testing instances.
        /// </summary>
        /// 
        ObservableCollection<Sample> Testing { get; }

        /// <summary>
        ///   Converts a input vector in feature representation into a Bitmap.
        /// </summary>
        /// 
        Bitmap ToBitmap(double[] features);

        /// <summary>
        ///   Converts a bitmap into a feature vector.
        /// </summary>
        ///  
        double[] ToFeatures(Bitmap bitmap);

        /// <summary>
        ///   Applies any preprocessing to feature vectors.
        /// </summary>
        /// 
        void Normalize(double[] features);

        /// <summary>
        ///   Gets or sets whether features should be normalized.
        /// </summary>
        /// 
        bool IsNormalized { get; set; }

        /// <summary>
        ///   Loads the training and testing samples.
        /// </summary>
        /// 
        void Load();
    }

    public static class DatabaseExtensions
    {
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            BitmapImage bitmapImage = null;

            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }

            bitmapImage.Freeze();

            return bitmapImage;
        }

        public static void GetInstances(this ObservableCollection<Sample> set, out double[][] input)
        {
            input = new double[set.Count][];
            for (int i = 0; i < input.Length; i++)
                input[i] = set[i].Features;
        }

        public static void GetInstances(this ObservableCollection<Sample> set, out double[][] input, out int[] output)
        {
            input = new double[set.Count][];
            output = new int[set.Count];
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = set[i].Features;
                output[i] = set[i].Class;
            }
        }

        public static void GetInstances(this ObservableCollection<Sample> set, out double[][] input, out double[][] output)
        {
            input = new double[set.Count][];
            output = new double[set.Count][];
            for (int i = 0; i < input.Length; i++)
            {
                int total = set[i].Database.Classes;

                input[i] = set[i].Features;
                output[i] = new double[total];
                output[i][set[i].Class] = 1;
            }
        }
    }
}
