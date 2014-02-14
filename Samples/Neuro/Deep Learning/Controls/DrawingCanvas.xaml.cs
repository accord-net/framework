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

namespace DeepLearning.Controls
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using AForge.Imaging.Filters;

    public partial class DrawingCanvas : UserControl
    {

        public object UserFeatures
        {
            get { return (object)GetValue(UserFeaturesProperty); }
            set { SetValue(UserFeaturesProperty, value); }
        }

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }


        public static readonly DependencyProperty UserFeaturesProperty =
          DependencyProperty.Register("UserFeatures", typeof(object), typeof(DrawingCanvas));

        public static readonly DependencyProperty IsActiveProperty =
          DependencyProperty.Register("IsActive", typeof(bool), typeof(DrawingCanvas));

      
        RenderTargetBitmap rtb;
        ResizeNearestNeighbor resize = new ResizeNearestNeighbor(32, 32);
        DispatcherTimer timer;

        public DrawingCanvas()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if ((bool)GetValue(IsActiveProperty))
                UserFeatures = getFeatures();
        }

        private double[] getFeatures()
        {
            rtb = new RenderTargetBitmap((int)InkCanvas1.ActualWidth, (int)InkCanvas1.ActualHeight,
                96d, 96d, PixelFormats.Default);

            rtb.Render(InkCanvas1);

            //save the ink to a memory stream
            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            byte[] bitmapBytes;
            Bitmap bmp;
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                ms.Position = 0;
                bitmapBytes = ms.ToArray();

                ms.Seek(0, SeekOrigin.Begin);
                bmp = new Bitmap(ms);
            }

            bmp = resize.Apply(bmp);

            double[] features = ToFeatures(bmp);

            return features;
        }

        public void Clear()
        {
            InkCanvas1.Strokes.Clear();
        }

        public static double[] ToFeatures(Bitmap bmp)
        {
            double[] features = new double[32 * 32];
            for (int i = 0; i < 32; i++)
                for (int j = 0; j < 32; j++)
                    features[i * 32 + j] = (bmp.GetPixel(j, i).R > 0) ? 0 : 1;

            return features;
        }
    }
}
