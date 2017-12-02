// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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

namespace Accord.Tests.IO
{
    using System;
    using System.IO;
    using System.Text;
    using Accord.IO;
    using Accord.Tests.MachineLearning.Properties;
    using NUnit.Framework;
    using Accord.Math;
    using System.Globalization;
    using Accord.MachineLearning.VectorMachines;
    using Accord.Tests.MachineLearning;
    using Accord.DataSets;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Statistics.Kernels;

#if NO_CULTURE
    using CultureInfo = Accord.Compat.CultureInfoEx;
#endif

#if NO_DEFAULT_ENCODING
    using Encoding = Accord.Compat.Encoding;
#endif

    [TestFixture]
    public class LibSvmModelTest
    {

        [Test]
        public void read_test()
        {
            string basePath = NUnit.Framework.TestContext.CurrentContext.TestDirectory;

            MemoryStream file = new MemoryStream(Encoding.Default.GetBytes(Resources.L1R_LR_a9a));
            TextReader reader = new StreamReader(file);
            File.WriteAllText(Path.Combine(basePath, "svm.txt"), reader.ReadToEnd());

            #region doc_read
            // Let's say we have used LIBLINEAR to learn a linear SVM model that has
            // been stored in a text file named "svm.txt". We would like to load this
            // same model in .NET and use it to make predictions using C#.
            // 
            // First, we use LibSvmModel.Load to load the LIBLINEAR model from disk:
            LibSvmModel model = LibSvmModel.Load(Path.Combine(basePath, "svm.txt"));

            // Now, we can use the model class to create the equivalent Accord.NET SVM:
            SupportVectorMachine svm = model.CreateMachine();

            // Now, we can use this machine normally, like as shown in the 
            // examples in the Support Vector Machine documentation page.
            #endregion

            Assert.AreEqual(2, svm.NumberOfClasses);
            Assert.AreEqual(122, svm.NumberOfInputs);
            Assert.AreEqual(1, svm.Weights.Length);
            Assert.AreEqual(1, svm.SupportVectors.Length);
        }

        [Test]
        public void ReadWriteTest_a9a()
        {
            string basePath = NUnit.Framework.TestContext.CurrentContext.TestDirectory;

            MemoryStream file = new MemoryStream(Encoding.Default.GetBytes(Resources.L1R_LR_a9a));

            LibSvmModel model1 = LibSvmModel.Load(file);
            string savePath = Path.Combine(basePath, "svm.txt");

            var svm1 = model1.CreateMachine();
            model1.Save(savePath);

            LibSvmModel model2 = LibSvmModel.Load(savePath);
            var svm2 = model2.CreateMachine();

            LibSvmModel model3 = LibSvmModel.FromMachine(svm1);
            var svm3 = model3.CreateMachine();
            model3.Solver = LibSvmSolverType.L1RegularizedLogisticRegression;

            string aPath = Path.Combine(basePath, "a.txt");
            string bPath = Path.Combine(basePath, "b.txt");
            string cPath = Path.Combine(basePath, "c.txt");
            model1.Save(aPath);
            model2.Save(bPath);
            model3.Save(cPath);

            string a = File.ReadAllText(aPath);
            string b = File.ReadAllText(bPath);
            string c = File.ReadAllText(cPath);

            Assert.AreEqual(a, b);
            Assert.AreEqual(a, c);


            Assert.AreEqual(svm1.Weights, svm2.Weights);
            Assert.AreEqual(svm1.SupportVectors, svm2.SupportVectors);
            Assert.AreEqual(svm1.Threshold, svm2.Threshold);

            Assert.AreEqual(svm1.Weights, svm3.Weights);
            Assert.AreEqual(svm1.SupportVectors, svm3.SupportVectors);
            Assert.AreEqual(svm1.Threshold, svm3.Threshold);
        }

        [Test]
        public void ReadLinearMachineTest()
        {
            MemoryStream file = new MemoryStream(
              Encoding.Default.GetBytes(Resources.L1R_LR_a9a));

            LibSvmModel reader = LibSvmModel.Load(file);

            Assert.AreEqual(-1, reader.Bias);
            Assert.AreEqual(2, reader.Classes);
            Assert.AreEqual(123, reader.Dimension);
            Assert.AreEqual(2, reader.Labels.Length);
            Assert.AreEqual(+1, reader.Labels[0]);
            Assert.AreEqual(-1, reader.Labels[1]);
            Assert.AreEqual(LibSvmSolverType.L1RegularizedLogisticRegression, reader.Solver);
            Assert.AreEqual(null, reader.Vectors);
            Assert.AreEqual(123, reader.Weights.Length);



            for (int i = 0; i < a9a_weights.Length; i++)
                Assert.AreEqual(a9a_weights[i], reader.Weights[i]);


            var machine = reader.CreateMachine();

            Assert.AreEqual(1, machine.SupportVectors.Length);
            Assert.AreEqual(122, machine.SupportVectors[0].Length);
            Assert.AreEqual(machine.Threshold, a9a_weights[0]);

            for (int i = 0; i < machine.SupportVectors[0].Length; i++)
                Assert.AreEqual(machine.SupportVectors[0][i], a9a_weights[i + 1]);
        }

        [Test]
        public void WriteLinearMachineTest()
        {
            MemoryStream destination = new MemoryStream();

            var model = new LibSvmModel()
            {
                Bias = -1,
                Classes = 2,
                Dimension = 123,
                Labels = new[] { +1, -1 },
                Solver = LibSvmSolverType.L1RegularizedLogisticRegression,
                Weights = a9a_weights
            };

            model.Save(destination);

            destination.Seek(0, SeekOrigin.Begin);
            TextReader textReader = new StreamReader(destination);
            string[] actual = textReader.ReadToEnd()
                .Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            string[] expected = Resources.L1R_LR_a9a
                .Split(new[] { "\r\n" }, StringSplitOptions.None);

            Assert.AreEqual(expected.Length, actual.Length);

            for (int i = 0; i < 7; i++)
                Assert.AreEqual(expected[i], actual[i]);

            for (int i = 6; i < expected.Length; i++)
            {
                if (expected[i] == actual[i])
                    continue;

                double a = Double.Parse(expected[i], CultureInfo.InvariantCulture);
                double b = Double.Parse(actual[i], CultureInfo.InvariantCulture);

                Assert.AreEqual(a, b);
            }

            Assert.AreEqual(expected[expected.Length - 1], String.Empty);
        }

        [Test]
        public void WriteLinearMachineTest_ExactCopy()
        {
            MemoryStream file = new MemoryStream(
              Encoding.Default.GetBytes(Resources.L1R_LR_a9a));

            LibSvmModel reader = LibSvmModel.Load(file);

            MemoryStream destination = new MemoryStream();
            reader.Save(destination);

            destination.Seek(0, SeekOrigin.Begin);

            TextReader textReader = new StreamReader(destination);
            string[] actual = textReader.ReadToEnd()
                .Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            string[] expected = Resources.L1R_LR_a9a
                .Split(new[] { "\r\n" }, StringSplitOptions.None);

            Assert.AreEqual(expected.Length, actual.Length);

            for (int i = 0; i < 7; i++)
                Assert.AreEqual(expected[i], actual[i]);

            for (int i = 6; i < expected.Length; i++)
            {
                if (expected[i] == actual[i])
                    continue;

                double a = Double.Parse(expected[i], CultureInfo.InvariantCulture);
                double b = Double.Parse(actual[i], CultureInfo.InvariantCulture);

                Assert.AreEqual(a, b);
            }

            Assert.AreEqual(expected[expected.Length - 1], String.Empty);
        }


        private static double[] a9a_weights =
        {
            -1.582078049646382,   -0.642353059177759,   -0.03749159485897963,  0.2771411674145598,
             0.2653433548837567,   0.3415826259468487,  -0.05175901290550707,  0.664475778321269,
             0.7785762379454263,   0.09938683894685095, -0.01638170625676196, -1.966943713947588,
             0,                   -0.1943487933572316,   0,                   -0.01427068979115089,
             0.03436343933761782,  0.04405623496590667, -0.2671859968627904,  -0.01805747891909384,
             0.5658635598535398,  -0.05370627691645281,  0.6313660889689086,   0.0203659899281737,
             0,                   -0.1356879782597286,  -0.05013455680484838,  0.8274477288905491,
             0,                    0,                    0.3329462841850907,   0.7437508445692492,
            -0.07194377091195173, -1.926282111405892,   -1.494061719727004,   -0.3166280869327708,
             0,                    0.1427422938386556,   1.007621381801709,    1.789552587610372,
            -0.4475053816996695,  -0.577595376635344,   -0.5775327047371792,   0.07589587177513718,
            -0.2633062199626867,   2.16187459176026,     0.7074073117375321,   0.1048275739725781,
            -0.7075406813675604,   0.3929609161252481,   0.8013002964263505,    0.6108089768685096,
            -0.6786424810841956,  -0.3491153276395321,   0.09966883995979201, -0.8531071757633746,
             0,                   -1.398866235553087,    0.7296056533697333,  -0.4887607820122212,
             1.078440711552346,   -0.7974653657893828,  -0.365703925070073,    0.227254295285581,
            -0.6760816771256906,   0,                    0.1009172420096765,   0.1545114041709269,
            -0.3633029234077111,  -0.3484744324185876,  -0.02191163808743305, -0.8402401543317437,
             0,                   -1.624966833470328,    0,                   -1.122080608688373,
             0,                   -1.220098499608685,   -0.4182725480858192,  -0.183241852284002,
             0.1221263566926127,   0.282442600832818,    0.3074189050166497,   0.7959947049068736,
             0.179689226126266,   -0.5067405511765133,   0.592363376246412,    0.6766998296161411,
            -0.991029010354986,   -0.2349170170278676,   0.5804018208787766,  -0.93788799387005,
            -0.5434195848637735,  -0.5901649721443204,   0.3803698957580355,   0.2590827574224139,
             0,                    0.7183509687151004,   0.8098991683873735,   0.08993105400479942,
            -0.07476049969988381, -0.9283947988462313,  -0.3739799928196428,   0.03171888330308115,
             1.197937572543021,    0.7364578149257212,  -1.349880912920405,   -0.131225067370858,
             0,                    0.1703791693215565,   0.2216059824256973,  -2.031437061283596,
             0,                   -0.4399798222194601,  -0.0728251038840361,   0,
             0,                    0.480264506314448,   -0.3699822088077383,  -0.1089542718979226,
            -1.247016957015914,    0,                    0,
        };
    }
}
