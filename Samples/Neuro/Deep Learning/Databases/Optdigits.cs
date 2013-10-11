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
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Media.Imaging;
    using Accord.Statistics;

    /// <summary>
    ///   Optdigits dataset from UCI Machine Learning Repository.
    /// </summary>
    /// 
    [Serializable]
    public class Optdigits : INotifyPropertyChanged, IDatabase
    {

        public int Classes { get { return 10; } }

        public ObservableCollection<Sample> Training { get; private set; }
        public ObservableCollection<Sample> Testing { get; private set; }


        double[] mean, dev;
        public bool IsNormalized { get; set; }

        public Optdigits()
        {
            mean = new double[1024];
            dev = new double[1024];
            for (int i = 0; i < dev.Length; i++)
                dev[i] = 1;
        }

        public void Load()
        {
            string input = Properties.Resources.optdigits_tra;

            // Load optdigits dataset into the DataGridView
            StringReader reader = new StringReader(input);

            Training = new ObservableCollection<Sample>();
            Testing = new ObservableCollection<Sample>();

            char[] buffer = new char[(32 + 2) * 32];
            int count = 0;

            while (true)
            {
                int read = reader.ReadBlock(buffer, 0, buffer.Length);
                string label = reader.ReadLine();

                if (read < buffer.Length || label == null)
                    break;


                ObservableCollection<Sample> set =
                    (count < 1000) ? Training : Testing;

                set.Add(extractSample(buffer, label));

                count++;
            }

            if (IsNormalized)
            {
                double[][] training;
                Training.GetInstances(out training);

                mean = training.Mean();
                dev = training.StandardDeviation();

                double[][] testing;
                Testing.GetInstances(out testing);

                Normalize(training);
                Normalize(testing);
            }
        }

        public void Normalize(double[][] inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = 0; j < inputs[i].Length; j++)
                {
                    inputs[i][j] -= mean[j];

                    if (dev[j] != 0)
                        inputs[i][j] /= dev[j];
                }
            }
        }

        public void Normalize(double[] inputs)
        {
            for (int j = 0; j < inputs.Length; j++)
            {
                inputs[j] -= mean[j];

                if (dev[j] != 0)
                    inputs[j] /= dev[j];

            }
        }

        private Sample extractSample(char[] buffer, string label)
        {
            Bitmap bitmap = Extract(new String(buffer));
            BitmapImage image = bitmap.ToBitmapImage();

            double[] features = ToFeatures(bitmap);
            int classLabel = Int32.Parse(label);

            return new Sample()
            {
                Database = this,

                Features = features,
                Image = image,

                Class = classLabel,

                Result = -1
            };
        }

        public Bitmap Extract(string text)
        {
            Bitmap bitmap = new Bitmap(32, 32, PixelFormat.Format32bppRgb);
            string[] lines = text.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    if (lines[i][j] == '0')
                        bitmap.SetPixel(j, i, Color.White);
                    else bitmap.SetPixel(j, i, Color.Black);
                }
            }
            return bitmap;
        }

        public double[] ToFeatures(Bitmap bmp)
        {
            double[] features = new double[32 * 32];
            for (int i = 0; i < 32; i++)
                for (int j = 0; j < 32; j++)
                    features[i * 32 + j] = (bmp.GetPixel(j, i).R > 0) ? 0 : 1;

            return features;
        }

        public Bitmap ToBitmap(double[] features)
        {
            if (features.Length != 1024)
                throw new Exception();

            Bitmap bitmap = new Bitmap(32, 32, PixelFormat.Format32bppRgb);

            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    int c = i * 32 + j;
                    double v = (features[c] * dev[c]) + mean[c];
                    v = Accord.Math.Tools.Scale(0, 1, 255, 0, v);
                    bitmap.SetPixel(j, i, Color.FromArgb((int)v, (int)v, (int)v));
                }
            }

            return bitmap;
        }


        // The PropertyChanged event doesn't needs to be explicitly raised
        // from this application. The event raising is handled automatically
        // by the NotifyPropertyWeaver VS extension using IL injection.
        //
#pragma warning disable 0067
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }
}
