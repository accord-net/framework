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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;
    using System;
    using System.Globalization;
    using Accord.Math;
    using Accord.Statistics.Visualizations;
    using System.Collections.Generic;

    [TestFixture]
    public class GammaDistributionTest
    {


        [Test]
        public void GammaDistributionConstructorTest()
        {
            double shape = 0.4;
            double scale = 4.2;

            double[] expected =
            {
                double.NegativeInfinity, 0.987114, 0.635929, 0.486871, 0.400046,
                0.341683, 0.299071, 0.266236, 0.239956, 0.218323, 0.200126
            };

            GammaDistribution target = new GammaDistribution(scale, shape);

            Assert.AreEqual(shape, target.Shape);
            Assert.AreEqual(scale, target.Scale);

            Assert.AreEqual(shape * scale, target.Mean);
            Assert.AreEqual(shape * scale * scale, target.Variance);
        }

        [Test]
        public void GammaDistributionConstructorTest2()
        {
            #region doc_ctor
            // Create a Γ-distribution with k = 2 and θ = 4
            var gamma = new GammaDistribution(theta: 4, k: 2);

            // Common measures
            double mean = gamma.Mean;     // 8.0
            double median = gamma.Median; // 6.7133878418421506
            double var = gamma.Variance;  // 32.0
            double mode = gamma.Mode;     // 4.0

            // Cumulative distribution functions
            double cdf = gamma.DistributionFunction(x: 0.27); // 0.002178158242390601
            double ccdf = gamma.ComplementaryDistributionFunction(x: 0.27); // 0.99782184175760935
            double icdf = gamma.InverseDistributionFunction(p: cdf); // 0.26999998689819171

            // Probability density functions
            double pdf = gamma.ProbabilityDensityFunction(x: 0.27); // 0.015773530285395465
            double lpdf = gamma.LogProbabilityDensityFunction(x: 0.27); // -4.1494220422235433

            // Hazard (failure rate) functions
            double hf = gamma.HazardFunction(x: 0.27); // 0.015807962529274005
            double chf = gamma.CumulativeHazardFunction(x: 0.27); // 0.0021805338793574793

            // String representation
            string str = gamma.ToString(CultureInfo.InvariantCulture); // "Γ(x; k = 2, θ = 4)"
            #endregion

            Assert.AreEqual(8.0, mean);
            Assert.AreEqual(6.7133878418421506, median, 1e-6);
            Assert.AreEqual(32.0, var);
            Assert.AreEqual(4.0, mode);
            Assert.AreEqual(0.0021805338793574793, chf, 1e-10);
            Assert.AreEqual(0.002178158242390601, cdf, 1e-10);
            Assert.AreEqual(0.015773530285395465, pdf, 1e-10);
            Assert.AreEqual(-4.1494220422235433, lpdf, 1e-10);
            Assert.AreEqual(0.015807962529274005, hf, 1e-10);
            Assert.AreEqual(0.99782184175760935, ccdf, 1e-10);
            Assert.AreEqual(0.26999998689819171, icdf, 1e-6);
            Assert.AreEqual("Γ(x; k = 2, θ = 4)", str);

            double p05 = gamma.DistributionFunction(median);
            Assert.AreEqual(0.5, p05, 1e-6);

            var range1 = gamma.GetRange(0.95);
            var range2 = gamma.GetRange(0.99);
            var range3 = gamma.GetRange(0.01);

            Assert.AreEqual(1.4214460427946485, range1.Min, 1e-10);
            Assert.AreEqual(18.975458073562308, range1.Max, 1e-10);
            Assert.AreEqual(0.59421896101306348, range2.Min, 1e-10);
            Assert.AreEqual(26.553408271975243, range2.Max, 1e-10);
            Assert.AreEqual(0.59421896101306348, range3.Min, 1e-10);
            Assert.AreEqual(26.553408271975243, range3.Max, 1e-10);

            Assert.AreEqual(0, gamma.Support.Min);
            Assert.AreEqual(double.PositiveInfinity, gamma.Support.Max);

            Assert.AreEqual(gamma.InverseDistributionFunction(0), gamma.Support.Min);
            Assert.AreEqual(gamma.InverseDistributionFunction(1), gamma.Support.Max);
        }

        [Test]
        public void MedianTest()
        {
            double shape = 0.4;
            double scale = 4.2;

            GammaDistribution target = new GammaDistribution(scale, shape);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }

        [Test]
        public void DensityFunctionTest()
        {
            double shape = 0.4;
            double scale = 4.2;

            double[] pdf =
            {
                double.PositiveInfinity, 0.987114, 0.635929, 0.486871, 0.400046,
                0.341683, 0.299071, 0.266236, 0.239956, 0.218323, 0.200126
            };

            GammaDistribution target = new GammaDistribution(scale, shape);

            for (int i = 0; i < 11; i++)
            {
                double x = i / 10.0;
                double actual = target.ProbabilityDensityFunction(x);
                double expected = pdf[i];

                Assert.AreEqual(expected, actual, 1e-6);
                Assert.IsFalse(double.IsNaN(actual));

                double logActual = target.LogProbabilityDensityFunction(x);
                double logExpected = Math.Log(pdf[i]);

                Assert.AreEqual(logExpected, logActual, 1e-5);
                Assert.IsFalse(double.IsNaN(logActual));
            }
        }

        [Test]
        public void CumulativeFunctionTest()
        {
            double shape = 0.4;
            double scale = 4.2;

            double[] cdf =
            {
                0, 0.251017, 0.328997, 0.38435, 0.428371, 0.465289,
                0.497226, 0.525426, 0.55069, 0.573571, 0.594469
            };

            GammaDistribution target = new GammaDistribution(scale, shape);

            for (int i = 0; i < 11; i++)
            {
                double x = i / 10.0;
                double actual = target.DistributionFunction(x);
                double expected = cdf[i];

                Assert.AreEqual(expected, actual, 1e-5);
                Assert.IsFalse(double.IsNaN(actual));
            }
        }

        [Test]
        public void GenerateTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            GammaDistribution target = new GammaDistribution(5, 2);

            double[] samples = target.Generate(10000000);

            var actual = GammaDistribution.Estimate(samples);

            Assert.AreEqual(5, actual.Scale, 1e-3);
            Assert.AreEqual(2, actual.Shape, 1e-3);
        }

        [Test]
        public void GenerateTest3()
        {
            Accord.Math.Tools.SetupGenerator(1);

            GammaDistribution target = new GammaDistribution(4, 2);

            double[] samples = new double[10000000];
            for (int i = 0; i < samples.Length; i++)
                samples[i] = target.Generate();

            var actual = GammaDistribution.Estimate(samples);

            Assert.AreEqual(4, actual.Scale, 5e-3);
            Assert.AreEqual(2, actual.Shape, 5e-3);
        }

        [Test]
        public void GenerateTest4()
        {
            Accord.Math.Tools.SetupGenerator(1);

            GammaDistribution target = new GammaDistribution(0.4, 0.2);

            double[] samples = new double[10000000];
            for (int i = 0; i < samples.Length; i++)
                samples[i] = target.Generate();

            var actual = GammaDistribution.Estimate(samples);

            Assert.AreEqual(0.4, actual.Scale, 1e-3);
            Assert.AreEqual(0.2, actual.Shape, 1e-3);
        }

        [Test]
        public void GenerateTest5()
        {
            Accord.Math.Tools.SetupGenerator(1);

            var target = new GammaDistribution(42, 0.1337);
            var samples = target.Generate(10000000);

            var actual = GammaDistribution.Estimate(samples);
            Assert.AreEqual(42, actual.Scale, 5e-2);
            Assert.AreEqual(0.1337, actual.Shape, 5e-4);
        }

        [Test]
        public void GenerateTest6()
        {
            // https://github.com/accord-net/framework/issues/281
            Accord.Math.Random.Generator.Seed = 1;

            var target = new GammaDistribution(1.5, 0.9);
            var samples = target.Generate(10000000);

            var actual = GammaDistribution.Estimate(samples);
            Assert.AreEqual(1.5, actual.Scale, 5e-2);
            Assert.AreEqual(0.9, actual.Shape, 5e-4);
        }


        [Test]
        public void NegativeValueTest()
        {
            double[] samples = NormalDistribution.Standard.Generate(100);

            try
            {
                GammaDistribution.Estimate(samples);
                Assert.Fail();
            }
            catch (ArgumentException)
            {

            }
        }


        [Test]
        public void FitTest()
        {
            // Gamma Distribution Fit stalls for some arrays #159
            // https://github.com/accord-net/framework/issues/159

            double[] x = { 1275.56, 1239.44, 1237.92, 1237.22, 1237.1, 1238.41, 1238.62, 1237.05, 1237.19, 1236.51, 1264.6, 1238.19, 1237.39, 1235.79, 1236.53, 1236.8, 1238.06, 1236.5, 1235.32, 1236.44, 1236.58, 1236.3, 1237.91, 1238.6, 1238.49, 1239.21, 1238.57, 1244.63, 1236.06, 1236.4, 1237.88, 1237.56, 1236.66, 1236.59, 1236.53, 1236.32, 1238.29, 1237.79, 1237.86, 1236.42, 1236.23, 1236.37, 1237.18, 1237.63, 1245.8, 1238.04, 1238.55, 1238.39, 1236.75, 1237.07, 1250.78, 1238.6, 1238.36, 1236.58, 1236.82, 1238.4, 1257.68, 1237.78, 1236.52, 1234.9, 1237.9, 1238.58, 1238.12, 1237.89, 1236.54, 1236.55, 1238.37, 1237.29, 1237.64, 1236.8, 1237.73, 1236.71, 1238.23, 1237.84, 1236.26, 1237.58, 1238.31, 1238.4, 1237.08, 1236.61, 1235.92, 1236.41, 1237.89, 1237.98, 1246.75, 1237.92, 1237.1, 1237.97, 1238.69, 1237.05, 1236.96, 1239.44, 1238.49, 1237.88, 1236.01, 1236.57, 1236.44, 1235.76, 1237.62, 1238, 1263.14, 1237.66, 1237, 1236, 1261.96, 1238.58, 1237.77, 1237.06, 1236.31, 1238.63, 1237.23, 1236.85, 1236.23, 1236.46, 1236.9, 1237.85, 1238, 1237.02, 1236.19, 1236.05, 1235.73, 1258.3, 1235.98, 1237.76, 1246.93, 1239.1, 1237.72, 1237.67, 1236.79, 1237.61, 1238.41, 1238.29, 1238.11, 1237, 1236.52, 1236.6, 1236.31, 1237.77, 1238.58, 1237.88, 1247.35, 1236.14, 1236.83, 1236.15, 1237.93, 1238.16, 1237.34, 1236.78, 1238.66, 1237.76, 1237.19, 1236.7, 1236.04, 1236.66, 1237.86, 1238.54, 1238.05, 1238.41, 1236.94, 1240.95, 1261.01, 1237.72, 1237.91, 1238.2, 1235.68, 1236.89, 1235.12, 1271.31, 1236.97, 1270.76, 1238.52, 1238.19, 1238.6, 1237.16, 1236.72, 1236.71, 1237.14, 1238.48, 1237.95, 1237.42, 1235.86, 1236.39, 1236.13, 1236.58, 1237.95, 1237.76, 1237.39, 1238.16, 1236.31, 1236.41, 1236.12, 1238.7, 1236.48, 1237.84, 1236.38, 1237.95, 1238.48, 1236.51, 1236.56 };

            var gamma = GammaDistribution.Estimate(x);

            Assert.AreEqual(1238.8734170854279, gamma.Mean, 1e-10);
            Assert.AreEqual(41566.439533445438, gamma.Shape, 1e-10);
            Assert.AreEqual(0.029804655654680219, gamma.Scale, 1e-10);
        }

        [Test]
        public void FitTest2()
        {
            // Gamma Distribution Fit stalls for some arrays #301
            // https://github.com/accord-net/framework/issues/301

            double[] x = { 1.003, 1.012, 1.011, 1.057, 1.033, 1.051, 1.045, 1.045, 1.037, 1.059, 1.028, 1.032, 1.029, 1.031, 1.029, 1.023, 1.035 };

            var gamma = GammaDistribution.Estimate(x);

            Assert.AreEqual(1.0329411764705885, gamma.Mean, 1e-10);
            Assert.AreEqual(4679.4730379075245, gamma.Shape, 1e-10);
            Assert.AreEqual(0.00022073878150444029, gamma.Scale, 1e-10);
        }

        [Test]
        public void FitTestOptions()
        {
            // Gamma Distribution Fit stalls for some arrays #301
            // https://github.com/accord-net/framework/issues/301

            double[] x = { 1.003, 1.012, 1.011, 1.057, 1.033, 1.051, 1.045, 1.045, 1.037, 1.059, 1.028, 1.032, 1.029, 1.031, 1.029, 1.023, 1.035 };

            var gamma = GammaDistribution.Estimate(x, tol: 1e-2, iterations: 0);

            Assert.AreEqual(1.0329411764705885, gamma.Mean, 1e-10);
            Assert.AreEqual(4679.4730319532555, gamma.Shape, 1e-10);
            Assert.AreEqual(0.00022073878178531338, gamma.Scale, 1e-10);
        }

        [Test]
        public void distribution_hangs()
        {
            // https://github.com/accord-net/framework/issues/304
            double[] dataset1 = { 8.141, 8.516, 7.049, 7.555, 7.999, 9.638, 7.445, 8.322, 8.184, 9.138, 8.489, 7.138, 7.855, 8.354, 10.648, 9.036, 9.371, 7.243, 6.967, 7.570, 8.636, 9.734, 8.713, 8.898, 7.969, 7.223, 7.162, 9.536, 8.919, 8.304, 7.746, 8.911, 7.857, 9.024, 7.383, 8.928, 7.410, 9.033, 7.912, 7.751, 7.359, 7.920, 7.294, 7.583, 8.122, 7.586, 7.011, 8.460, 9.126, 7.247, 9.816, 8.411, 8.833, 9.028, 8.271, 7.698, 7.399, 9.823, 8.202, 8.413, 7.384, 7.721, 7.384, 7.660, 7.698, 8.396, 8.038, 7.295, 8.202, 9.214, 10.263, 7.990, 7.502, 8.502, 8.022, 7.824, 9.193, 9.490, 9.279, 8.961, 8.213, 7.448, 8.385, 9.276, 8.178, 7.102, 7.206, 7.594, 7.718, 7.770, 7.997, 7.866, 8.157, 9.319, 7.449, 8.559, 7.617, 8.074, 7.922, 9.178, 8.738, 7.679, 7.983, 8.307, 8.022, 9.407, 7.667, 8.844, 8.134, 8.383, 8.395, 8.004, 9.252, 8.130, 7.934, 8.140, 7.924, 8.893, 6.850, 8.238, 9.762, 7.544, 8.845, 7.595, 8.453, 7.892, 8.007, 8.352, 8.537, 9.402 };
            double[] dataset2 = { 0.999, 0.996, 1.066, 1.041, 1.052, 1.043, 1.052, 1.046, 1.032, 1.038, 1.052, 1.038, 1.040, 1.056, 1.043, 1.044, 1.058, 1.044, 1.053, 1.046, 1.045, 1.046, 1.048, 1.051, 1.056, 1.058, 1.065, 1.051, 1.049, 1.049, 1.043, 1.042, 1.049, 1.042, 1.033, 1.042, 1.028, 1.031, 1.036, 1.044, 1.037, 1.054, 1.046, 1.061, 1.048, 1.048, 1.028, 1.030, 1.049, 1.038, 1.042, 1.057, 1.028, 1.047, 1.044, 1.040, 1.038, 1.044, 1.046, 1.035, 1.040, 1.039, 1.046, 1.050, 1.033, 1.032, 1.040, 1.040, 1.032, 1.045, 1.054, 1.040, 1.045, 1.045, 1.043, 1.066, 1.057, 1.052, 1.060, 1.069, 1.046, 1.041, 1.048, 1.057, 1.059, 1.076, 1.051, 1.057, 1.067, 1.062, 1.025, 1.072, 1.061, 1.067, 1.062, 1.055, 1.065, 1.047, 1.046, 1.047, 1.045, 1.038, 1.056, 1.037, 1.042, 1.044, 1.047, 1.037, 1.047, 1.041, 1.042, 1.042, 1.042, 1.040, 1.060, 1.066, 1.067, 1.077, 1.059, 1.067 };

            // with Accord.NET the results of the gamma fit are
            // Dataset1: k = 119.306665626699, θ = 0.0690958879514208  (rate=1/θ = 14.4726412764688)
            // Dataset2: k = 6952.29276909337, θ = 0.00015060019786871 (rate=1/θ = 6640.0975174798) 

            // with the same data a gamma distribution fit was performed online via "http://www.wessa.net/rwasp_fitdistrgamma.wasp"
            // Dataset1: shape= 115.405346712493, rate = 13.9994954351296 (θ= 1/shape= 0.07143114583191708605319342679921)
            // Dataset2: shape= 6930.15592532156 , rate = 6618.95473828367 (θ= 1/shape= 0.00015108125671506031378872360567992)

            var a = GammaDistribution.Estimate(dataset1);
            Assert.AreEqual(0.069095887951420826, a.Scale, 1e-8);
            Assert.AreEqual(119.30666562669866, a.Shape, 1e-8);

            var b = GammaDistribution.Estimate(dataset2);
            Assert.AreEqual(0.00015060019786871043, b.Scale, 1e-8);
            Assert.AreEqual(6952.2927690933739, b.Shape, 1e-8);


            double[] pdfa = dataset1.Apply(a.ProbabilityDensityFunction);
            double[] pdfb = dataset2.Apply(b.ProbabilityDensityFunction);

            double[] ay = plot(dataset1, a);
            string stra = ay.ToCSharp();
            double[] expecteda = new double[] { 0.0927186751703724, 0.102929217255775, 0.113853912640037, 0.125490882412066, 0.137831384027626, 0.150859382782722, 0.164551205816893, 0.178875290227262, 0.193792035506903, 0.209253768840681, 0.225204829830127, 0.241581779018976, 0.25831373220303, 0.275322819989417, 0.292524769484354, 0.309829602400448, 0.327142441352633, 0.344364413721372, 0.36139364026723, 0.378126293740165, 0.394457711091658, 0.410283541612013, 0.425500912411485, 0.440009592166544, 0.453713133972042, 0.466519978477992, 0.478344499233706, 0.489107973291516, 0.498739461604722, 0.507176585549156, 0.514366187957508, 0.520264869325212, 0.524839392269736, 0.528066949839688, 0.529935295816686, 0.530442737670986, 0.529597995264547, 0.527419930690551, 0.523937156748862, 0.51918753344197, 0.513217563503365, 0.50608169931424, 0.497841574608112, 0.488565175098291, 0.478325962588825, 0.467201967252403, 0.455274862593154, 0.442629037177336, 0.429350676535845, 0.415526867748883, 0.401244738146801, 0.386590638336579, 0.37164937842665, 0.356503524908677, 0.341232764198471, 0.325913337372842, 0.310617549195024, 0.29541335312659, 0.280364012703018, 0.265527838423449, 0.250957998190604, 0.236702398346326, 0.222803631491404, 0.209298986560572, 0.196220516046194, 0.183595154826475, 0.171444884751058, 0.159786938962256, 0.148634039874823, 0.137994664790792, 0.127873333276316, 0.118270910662634, 0.109184922339614, 0.100609873875035, 0.0925375724026959, 0.0849574451652891, 0.0778568515617006, 0.0712213855225515, 0.0650351655120983, 0.0592811099206679, 0.0539411960619302, 0.0489967014173125, 0.0444284261704561, 0.0402168964441056, 0.0363425479871147, 0.0327858903587122, 0.0295276519198202, 0.0265489061670697, 0.0238311801347997, 0.0213565457451675, 0.0191076951082055, 0.0170680008645135, 0.0152215627257103, 0.0135532414045203, 0.012048681140193, 0.0106943220187805, 0.00947740326439647, 0.00838595863975468, 0.00740880504465858, 0.00653552534217913 };
            Assert.IsTrue(ay.IsEqual(expecteda.Log(), 1e-8));

            double[] by = plot(dataset2, b);
            string strb = by.ToCSharp();
            double[] expectedb = new double[] { -5.02287583851557, -4.75049252465396, -4.48269919286031, -4.21948839264951, -3.96085269165997, -3.7067846755981, -3.45727694817651, -3.21232213106123, -2.97191286381076, -2.73604180381335, -2.50470162623969, -2.27788502397743, -2.05558470757569, -1.8377934051905, -1.62450386252749, -1.41570884278372, -1.21140112659032, -1.01157351196434, -0.816218814241438, -0.62532986603037, -0.438899517150276, -0.256920634583366, -0.079386102411263, 0.0937111782322972, 0.262378289222397, 0.426622295487505, 0.586450245075866, 0.741869169200072, 0.892886082292534, 1.03950798206188, 1.18174184954296, 1.31959464915144, 1.45307332873381, 1.58218481962467, 1.70693603669679, 1.82733387840926, 1.94338522686394, 2.05509694785906, 2.16247589093564, 2.26552888942933, 2.36426276052498, 2.45868430530481, 2.5488003087994, 2.63461754003947, 2.71614275210504, 2.79338268217543, 2.86634405157747, 2.93503356584097, 2.99945791474238, 3.05962377235392, 3.11553779709811, 3.16720663179149, 3.21463690369819, 3.25783522457004, 3.29680819070381, 3.33156238298761, 3.36210436694364, 3.38844069278093, 3.41057789544175, 3.42852249464886, 3.44228099495103, 3.45185988577396, 3.45726564146389, 3.45850472133588, 3.45558356971742, 3.44850861599934, 3.43728627467954, 3.42192294540746, 3.40242501303237, 3.37879884764789, 3.35105080463927, 3.31918722472255, 3.28321443399909, 3.24313874399377, 3.19896645169865, 3.15070383962393, 3.0983571758361, 3.04193271400709, 2.98143669345427, 2.91687533918594, 2.84825486194768, 2.77558145825969, 2.69886131046587, 2.61810058677565, 2.53330544130586, 2.44448201412342, 2.35163643128908, 2.2547748049019, 2.15390323313659, 2.04902780028988, 1.94015457682235, 1.82728961939847, 1.71043897092841, 1.5896086606117, 1.4648047039791, 1.33603310292983, 1.20329984577529, 1.06661090728176, 0.925972248708604, 0.781389817848321 };
            Assert.IsTrue(by.IsEqual(expectedb, 1e-8));

            double[] x = Vector.Range(0.0, 10.0, 0.05);
            double[] pdf = x.Apply(b.LogProbabilityDensityFunction).Exp();
            string str = pdf.ToCSharp();
            double[] expected = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1.60168777623766E-293, 5.51885646801681E-214, 6.87229105720087E-150, 3.05548162854266E-99, 2.07661256908294E-60, 4.84694366550593E-32, 5.26688946739727E-13, 0.0241774294264879, 30.7996636820675, 0.0054993586504859, 5.60031174239992E-13, 1.10634606299171E-28, 1.24123786224247E-49, 2.04031751961623E-75, 1.13865913049715E-105, 4.56124541509388E-140, 2.56251968244235E-178, 3.68510891441049E-220, 2.33359893081555E-265, 1.06302359249589E-313, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };;
            Assert.IsTrue(pdf.IsEqual(expected, 1e-8));
        }

        private static double[] plot(double[] dataset1, GammaDistribution a)
        {
            var hist = new Histogram();
            hist.Compute(dataset1);

            double x = hist.Range.Min;
            double stepx = (hist.Range.Max - hist.Range.Min) / 100;

            int size = 100;
            var ax = new double[size];
            var ay = new double[size];
            for (int i = 0; i < size; i++)
            {
                double y = a.ProbabilityDensityFunction(x);
                double ly = a.LogProbabilityDensityFunction(x);

                Assert.AreEqual(y, Math.Exp(ly), 1e-10);
                Assert.AreEqual(Math.Log(y), ly, 1e-10);

                x = x + stepx;
                ax[i] = x;
                ay[i] = ly;
            }

            return ay;
        }
    }
}