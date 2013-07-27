using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accord.Imaging.Filters;
using Accord.Imaging;
using Accord.Controls;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing;

namespace Accord.Tests.Imaging
{
    /// <summary>
    /// Summary description for BlendTest
    /// </summary>
    [TestClass]
    public class ProjectiveTransformTest
    {
        public ProjectiveTransformTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void ApplyTest()
        {
            Bitmap img1 = Properties.Resources.image2;


            MatrixH homography = new MatrixH(1, 0, 32,
                                              0, 1, 0,
                                              0, 0);

            ProjectiveTransform transform = new ProjectiveTransform(homography);
            transform.FillColor = Color.Red;
            Bitmap actual = transform.Apply(img1);

            ImageBox.Show(actual, PictureBoxSizeMode.Zoom);

            Assert.AreEqual(64, actual.Size.Width);
            Assert.AreEqual(32, actual.Size.Height);



            homography = new MatrixH(2, 0, 0,
                                     0, 2, 0,
                                     0, 0);

            transform = new ProjectiveTransform(homography);
            transform.FillColor = Color.Red;
            actual = transform.Apply(img1);

            ImageBox.Show(actual, PictureBoxSizeMode.Zoom);

            Assert.AreEqual(32, actual.Size.Width);
            Assert.AreEqual(32, actual.Size.Height);



            homography = new MatrixH(2, 0, 32,
                                     0, 0.5f, 32,
                                     0, 0);

            transform = new ProjectiveTransform(homography);
            transform.FillColor = Color.Red;
            actual = transform.Apply(img1);

            ImageBox.Show(actual, PictureBoxSizeMode.Zoom);

            Assert.AreEqual(32 / 2 + 32, actual.Size.Width);
            Assert.AreEqual(32 / 0.5 + 32, actual.Size.Height);



            homography = new MatrixH(1, 0, -32,
                                     0, 1,   0,
                                     0, 0);

            transform = new ProjectiveTransform(homography);
            transform.FillColor = Color.Red;
            actual = transform.Apply(img1);

            ImageBox.Show(actual, PictureBoxSizeMode.Zoom);

            Assert.AreEqual(64, actual.Size.Width);
            Assert.AreEqual(32, actual.Size.Height);
        }

    }
}
