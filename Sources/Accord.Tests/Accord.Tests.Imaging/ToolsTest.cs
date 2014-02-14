// Accord Unit Tests
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

using Accord.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accord.Math;
using System.Drawing;

using Tools = Accord.Imaging.Tools;
using System.Drawing.Imaging;
using Accord.Math.Decompositions;
using System;
using AForge.Imaging;
using Accord.Imaging.Converters;

namespace Accord.Tests.Imaging
{


    [TestClass()]
    public class ToolsTest
    {

        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }



        [TestMethod()]
        public void NormalizeTest()
        {
            PointH[] points = new PointH[] 
            {
                new PointH(1, 2),
                new PointH(5, 2),
                new PointH(12, 2),
                new PointH(1, 2),
                new PointH(10, 2),
            };

            MatrixH T;
            PointH[] actual = Tools.Normalize(points, out T);


            // Centroids should be at the origin
            double cx = 0, cy = 0;
            for (int i = 0; i < actual.Length; i++)
            {
                cx += actual[i].X / actual[i].W;
                cy += actual[i].Y / actual[i].W;
            }
            Assert.AreEqual(cx / actual.Length, 0, 0.0000001);
            Assert.AreEqual(cy / actual.Length, 0, 0.0000001);

            // Average distance from the origin should be sqrt(2)
            double d = 0;
            for (int i = 0; i < actual.Length; i++)
            {
                double x = actual[i].X / actual[i].W;
                double y = actual[i].Y / actual[i].W;

                d += System.Math.Sqrt(x * x + y * y);
            }
            Assert.AreEqual(d / actual.Length, System.Math.Sqrt(2), 0.00001);


        }

        [TestMethod()]
        public void MultiplyTest()
        {
            MatrixH matrix = new MatrixH(Matrix.Identity(3));

            PointH[] points = new PointH[] 
            {
                new PointH(1, 2),
                new PointH(5, 2),
                new PointH(12, 2),
                new PointH(1, 2),
                new PointH(10, 2),
            };


            PointH[] expected = new PointH[] 
            {
                new PointH(1, 2),
                new PointH(5, 2),
                new PointH(12, 2),
                new PointH(1, 2),
                new PointH(10, 2),
            };

            PointH[] actual = (PointH[])points.Clone();
            matrix.TransformPoints(actual);

            Assert.AreEqual(expected[0], actual[0]);
            Assert.AreEqual(expected[1], actual[1]);
            Assert.AreEqual(expected[2], actual[2]);
            Assert.AreEqual(expected[3], actual[3]);
            Assert.AreEqual(expected[4], actual[4]);

        }

        [TestMethod()]
        public void ColinearTest()
        {
            bool actual;

            PointH p1 = new PointH(0, 1);
            PointH p2 = new PointH(0, 2);
            PointH p3 = new PointH(0, 3);

            bool expected = true;
            actual = Tools.Collinear(p1, p2, p3);
            Assert.AreEqual(expected, actual);


            p1 = new PointH(0, 1);
            p2 = new PointH(1, 0);
            p3 = new PointH(1, 1);

            expected = false;
            actual = Tools.Collinear(p1, p2, p3);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod()]
        public void HomographyTest()
        {
            PointH[] x1 = 
            {
                new PointH(0, 0),
                new PointH(1, 0),
                new PointH(0, 1),
                new PointH(1, 1),
            };

            PointH[] x2 = 
            {
                new PointH(0, 0),
                new PointH(1, 0),
                new PointH(0, 1),
                new PointH(1, 1),
            };

            double[,] expected = Matrix.Identity(3);

            double[,] actual = (double[,])Tools.Homography(x1, x2);

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    actual[i, j] /= actual[2, 2];

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-4));


            x1 = new PointH[] 
            {
                new PointH(2, 0),
                new PointH(1, 0),
                new PointH(5, 1),
                new PointH(1, 1),
                new PointH(7, 1),
                new PointH(1, 2),
                new PointH(1, 1),
            };

            x2 = new PointH[] 
            {
                new PointH(9, 1),
                new PointH(1, 5),
                new PointH(9, 1),
                new PointH(1, 7),
                new PointH(2, 7),
                new PointH(6, 5),
                new PointH(1, 7),
            };

            expected = new double[,]
            {
              { 0.2225, -3.1727,  1.8023 },
              { 0.3648, -1.7149, -0.2173 },
              { 0.0607, -0.4562,  0.1229 },
            };

            expected = (double[,])(new MatrixH(expected));

            actual = (double[,])Tools.Homography(x1, x2);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.01));

        }

        [TestMethod()]
        public void HomographyTestF()
        {
            PointF[] x1 = 
            {
                new PointF(0, 0),
                new PointF(1, 0),
                new PointF(0, 1),
                new PointF(1, 1),
            };

            PointF[] x2 = 
            {
                new PointF(0, 0),
                new PointF(1, 0),
                new PointF(0, 1),
                new PointF(1, 1),
            };

            float[,] expected = Matrix.Identity(3).ToSingle();

            float[,] actual = (float[,])Tools.Homography(x1, x2);

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    actual[i, j] /= actual[2, 2];


            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-5));


            x1 = new PointF[]
            {
                new PointF(2, 0),
                new PointF(1, 0),
                new PointF(5, 1),
                new PointF(1, 1),
                new PointF(7, 1),
                new PointF(1, 2),
                new PointF(1, 1),
            };

            x2 = new PointF[]
            {
                new PointF(9, 1),
                new PointF(1, 5),
                new PointF(9, 1),
                new PointF(1, 7),
                new PointF(2, 7),
                new PointF(6, 5),
                new PointF(1, 7),
            };

            expected = new float[,]
            {
                { 0.2225f, -3.1727f,  1.8023f },
                { 0.3648f, -1.7149f, -0.2173f },
                { 0.0607f, -0.4562f,  0.1229f },
            };

            expected = (float[,])(new MatrixH(expected));

            actual = (float[,])Tools.Homography(x1, x2);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.01));

        }

        [TestMethod()]
        public void ToDoubleArrayTest()
        {
            Bitmap image = Properties.Resources.image1;

            double[] actual = Tools.ToDoubleArray(image, 0, 0, 1);


            for (int i = 0; i < 256; i++)
            {
                if (i == 2 * 16 + 2 || i == 2 * 16 + 13 ||
                    i == 13 * 16 + 2 || i == 13 * 16 + 13)
                    Assert.AreEqual(0, actual[i]);
                else
                    Assert.AreNotEqual(0, actual[i]);
            }

        }


        [TestMethod()]
        public void ColinearTest1()
        {
            bool actual;
            PointF pt1, pt2, pt3;

            pt1 = new PointF(0, 0);
            pt2 = new PointF(1, 1);
            pt3 = new PointF(2, 2);

            actual = Tools.Collinear(pt1, pt2, pt3);
            Assert.AreEqual(true, actual);


            pt1 = new PointH(0, 1);
            pt2 = new PointH(1, 1);
            pt3 = new PointH(2, 2);

            actual = Tools.Collinear(pt1, pt2, pt3);
            Assert.AreEqual(false, actual);
        }

        [TestMethod()]
        public void SumTest()
        {
            Bitmap image = Properties.Resources.image1;

            int expected = 0;

            for (int i = 0; i < 16; i++)
                for (int j = 0; j < 16; j++)
                    expected += image.GetPixel(i, j).R;

            int actual = (int)Tools.Sum(image);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void SumTest1()
        {
            Bitmap image = Properties.Resources.image2;

            int expected = 0;

            for (int i = 3; i < 9; i++)
                for (int j = 2; j < 10; j++)
                    expected += image.GetPixel(i, j).R;


            BitmapData data = image.LockBits(new Rectangle(Point.Empty, image.Size),
                ImageLockMode.ReadOnly, image.PixelFormat);

            int actual = Tools.Sum(data, new Rectangle(3, 2, 9 - 3, 10 - 2));
            Assert.AreEqual(expected, actual);

            image.UnlockBits(data);
        }

        [TestMethod()]
        public void SumTest2()
        {
            Bitmap image = Properties.Resources.image2;

            int expected = 0;

            for (int i = 3; i < 9; i++)
                for (int j = 2; j < 10; j++)
                    expected += image.GetPixel(i, j).R;

            int actual = (int)Tools.Sum(image, new Rectangle(3, 2, 9 - 3, 10 - 2));
            Assert.AreEqual(expected, actual);

        }

        [TestMethod()]
        public void ToBitmapTest1()
        {
            double[][] pixels = 
            {
                new double[] { 0,0,0 }, new double[] { 0,0,1 }, new double[] { 0,1,0 },
                new double[] { 0,1,1 }, new double[] { 1,0,0 }, new double[] { 1,0,1 },
                new double[] { 1,1,0 }, new double[] { 1,1,1 }, new double[] { 0,0.5,0 },
            };

            int width = 3;
            int height = 3;
            double min = 0;
            double max = 1;


            Bitmap actual = Tools.ToBitmap(pixels, width, height, min, max);

            // Accord.Controls.ImageBox.Show(actual, PictureBoxSizeMode.Zoom);

            Assert.AreEqual(Color.FromArgb(000, 000, 000), actual.GetPixel(0, 0));
            Assert.AreEqual(Color.FromArgb(000, 000, 255), actual.GetPixel(1, 0));
            Assert.AreEqual(Color.FromArgb(000, 255, 000), actual.GetPixel(2, 0));

            Assert.AreEqual(Color.FromArgb(000, 255, 255), actual.GetPixel(0, 1));
            Assert.AreEqual(Color.FromArgb(255, 000, 000), actual.GetPixel(1, 1));
            Assert.AreEqual(Color.FromArgb(255, 000, 255), actual.GetPixel(2, 1));

            Assert.AreEqual(Color.FromArgb(255, 255, 000), actual.GetPixel(0, 2));
            Assert.AreEqual(Color.FromArgb(255, 255, 255), actual.GetPixel(1, 2));
            Assert.AreEqual(Color.FromArgb(000, 127, 000), actual.GetPixel(2, 2));

            double[][] actualArray = actual.ToDoubleArray(min, max);
            Assert.IsTrue(Matrix.IsEqual(pixels, actualArray, 0.01));

        }

        [TestMethod()]
        public void ToBitmapTest()
        {
            double[] array = 
            {
                0,0,0,
                0,1,0,
                0,0,0,
            };

            int width = 3;
            int height = 3;
            double min = 0;
            double max = 1;

            Bitmap actual = Accord.Imaging.Tools.ToBitmap(array, width, height, min, max);

            double[] actualarray = actual.ToDoubleArray(0, min, max);

            Assert.IsTrue(Matrix.IsEqual(array, actualarray));

        }


        [TestMethod()]
        public void MeanTest()
        {
            Bitmap image = new byte[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 },
            }.ToBitmap();

            {
                Rectangle rectangle = new Rectangle(0, 0, 1, 2);
                double expected = (1 + 4) / 2.0;
                double actual = Tools.Mean(image, rectangle);
                Assert.AreEqual(expected, actual);
            }

            {
                double expected = (1 + 2 + 3 + 4 + 5 + 6 + 7 + 8 + 9) / 9.0;
                double actual = Tools.Mean(image);
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod()]
        public void MeanTest2()
        {
            // Test for 16 bpp images
            Bitmap image = new short[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 },
            }.ToBitmap();

            {
                Rectangle rectangle = new Rectangle(0, 0, 1, 2);
                double expected = (1 + 4) / 2.0;
                double actual = Tools.Mean(image, rectangle);
                Assert.AreEqual(expected, actual);
            }

            {
                double expected = (1 + 2 + 3 + 4 + 5 + 6 + 7 + 8 + 9) / 9.0;
                double actual = Tools.Mean(image);
                Assert.AreEqual(expected, actual);
            }
        }


        [TestMethod()]
        public void MeanTest3()
        {
            UnmanagedImage image = UnmanagedImage.FromManagedImage(new byte[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 },
            }.ToBitmap());

            {
                Rectangle rectangle = new Rectangle(0, 0, 1, 2);
                double expected = (1 + 4) / 2.0;
                double actual = Tools.Mean(image, rectangle);
                Assert.AreEqual(expected, actual);
            }

            {
                double expected = (1 + 2 + 3 + 4 + 5 + 6 + 7 + 8 + 9) / 9.0;
                double actual = Tools.Mean(image);
                Assert.AreEqual(expected, actual);
            }
        }



        [TestMethod()]
        public void StandardDeviationTest()
        {
            double[] values = { 5, 2, 7, 5, 3, 5, 1, 1, 2 };

            Bitmap image = values.ToBitmap(3, 3, 0, 255);
            double mean = Accord.Statistics.Tools.Mean(values);
            double expected = Accord.Statistics.Tools.StandardDeviation(values);
            double actual = Tools.StandardDeviation(image, mean);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void StandardDeviationTest2()
        {
            short[,] values =
            {
                { 5, 2, 7 },
                { 5, 3, 5 },
                { 1, 1, 2 }
            };

            Bitmap image = values.ToBitmap();
            double mean = Accord.Statistics.Tools.Mean(values.Reshape().ToDouble());
            double expected = Accord.Statistics.Tools.StandardDeviation(values.Reshape().ToDouble());
            double actual = Tools.StandardDeviation(image, mean);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void StandardDeviationTest3()
        {
            short[,] values =
            {
                { 5, 2, 7 },
                { 5, 3, 5 },
                { 1, 1, 2 }
            };

            Rectangle rect = new Rectangle(1, 1, 2, 1);

            Bitmap image = values.ToBitmap();
            double mean = Accord.Statistics.Tools.Mean(new double[] { 3, 5 });
            double expected = Accord.Statistics.Tools.StandardDeviation(new double[] { 3, 5 });
            double actual = Tools.StandardDeviation(image, rect, mean);
            Assert.AreEqual(expected, actual);
        }



        [TestMethod()]
        public void MaxTest2()
        {
            Bitmap image = new byte[,]
            {
                { 5, 2, 7 },
                { 5, 3, 5 },
                { 9, 1, 2 }
            }.ToBitmap();

            {
                Rectangle rectangle = new Rectangle(1, 0, 2, 2);
                int expected = 7;
                int actual = Tools.Max(image, rectangle);
                Assert.AreEqual(expected, actual);
            }
            {
                int expected = 9;
                int actual = Tools.Max(image);
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod()]
        public void MinTest2()
        {
            Bitmap image = new byte[,]
            {
                { 5, 2, 7 },
                { 5, 3, 5 },
                { 9, 1, 2 }
            }.ToBitmap();

            {
                Rectangle rectangle = new Rectangle(1, 0, 2, 2);
                int expected = 2;
                int actual = Tools.Min(image, rectangle);
                Assert.AreEqual(expected, actual);
            }
            {
                int expected = 1;
                int actual = Tools.Min(image);
                Assert.AreEqual(expected, actual);
            }
        }


    }
}
