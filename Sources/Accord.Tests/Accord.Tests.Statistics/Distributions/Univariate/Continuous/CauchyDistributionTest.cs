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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Distributions.Univariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Controls;

    [TestClass()]
    public class CauchyDistributionTest
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
        public void CauchyDistributionConstructorTest()
        {
            double location = 2;
            double scale = 4;
            CauchyDistribution target = new CauchyDistribution(location, scale);

            Assert.AreEqual(location, target.Location);
            Assert.AreEqual(scale, target.Scale);

            Assert.AreEqual(location, target.Median);
            Assert.AreEqual(location, target.Mode);
            Assert.IsTrue(Double.IsNaN(target.Mean));
            Assert.IsTrue(Double.IsNaN(target.Variance));
            Assert.AreEqual(Math.Log(scale) + Math.Log(4.0 * Math.PI), target.Entropy);
        }

        [TestMethod()]
        public void MedianTest()
        {
            double location = 2;
            double scale = 4;
            CauchyDistribution target = new CauchyDistribution(location, scale);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5), 1e-10);
        }

        [TestMethod()]
        public void CauchyDistributionConstructorTest1()
        {
            CauchyDistribution target = new CauchyDistribution();

            Assert.AreEqual(0, target.Location);
            Assert.AreEqual(1, target.Scale);
        }


        [TestMethod()]
        public void DistributionFunctionTest()
        {
            double[] expected = 
            {
                0.7729505, 0.7931890, 0.8105287, 0.8254671, 0.8384167, 0.8497145, 
                0.8596339, 0.8683966, 0.8761824, 0.8831381, 0.8893841, 0.8950196
            };

            CauchyDistribution target = new CauchyDistribution(location: -7.2, scale: 6.23);

            for (int i = 0; i < expected.Length; i++)
            {
                double actual = target.DistributionFunction(i);
                Assert.AreEqual(expected[i], actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));
            }
        }

        [TestMethod()]
        public void FitTest()
        {
            CauchyDistribution target = new CauchyDistribution();
            double[] observations = samples;

            target.Fit(observations);


            Assert.AreEqual(4.2, target.Location, 0.3);
            Assert.AreEqual(0.21, target.Scale, 0.05);
        }

        [TestMethod()]
        public void FitTest2()
        {
            double[] observations = { 0.25, 0.12, 0.72, 0.21, 0.62, 0.12, 0.62, 0.12 };

            {
                CauchyDistribution cauchy = new CauchyDistribution();

                cauchy.Fit(observations);

                Assert.AreEqual(0.18383597286086659, cauchy.Location);
                Assert.AreEqual(-0.10530822112775458, cauchy.Scale);
            }
            {
                CauchyOptions options = new CauchyOptions()
                {
                    EstimateLocation = true,
                    EstimateScale = false
                };


                CauchyDistribution cauchy = new CauchyDistribution(location: 0, scale: 4.2);

                cauchy.Fit(observations, options);

                Assert.AreEqual(0.34712181102025652, cauchy.Location);
                Assert.AreEqual(4.2, cauchy.Scale);
            }
        }

        [TestMethod()]
        public void LogProbabilityDensityFunctionTest()
        {
            double[] expected = 
            {
                -3.4572653, -3.2488640, -3.0165321, -2.7541678, -2.4530627, -2.1002413,
                -1.6753581, -1.1447299, -0.4515827,  0.4647080,  1.1578552,  0.4647080
            };

            CauchyDistribution target = new CauchyDistribution(location: 1, scale: 0.1);

            for (int i = 0; i < expected.Length; i++)
            {
                double actual = target.LogProbabilityDensityFunction(i / 10.0);
                Assert.AreEqual(expected[i], actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));
            }
        }

        [TestMethod()]
        public void ProbabilityDensityFunctionTest()
        {
            double[] expected = 
            {
                0.03183099, 0.03314002, 0.03452385, 0.03598755, 0.03753654, 0.03917660,
                0.04091387, 0.04275485, 0.04470644, 0.04677588, 0.04897075, 0.05129893
            };

            CauchyDistribution target = new CauchyDistribution(location: 4, scale: 2);

            for (int i = 0; i < expected.Length; i++)
            {
                double actual = target.ProbabilityDensityFunction(i / 10.0);
                Assert.AreEqual(expected[i], actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));
            }
        }

        [TestMethod()]
        public void CauchyDistributionConstructorTest2()
        {
            double location = 0.42;
            double scale = 1.57;

            CauchyDistribution cauchy = new CauchyDistribution(location, scale);

            double mean = cauchy.Mean; // NaN - Cauchy's mean is undefined.
            double var = cauchy.Variance; // NaN - Cauchy's variance is undefined.
            double median = cauchy.Median; // 0.42

            double cdf = cauchy.DistributionFunction(x: 0.27); // 0.46968025841608563
            double pdf = cauchy.ProbabilityDensityFunction(x: 0.27); // 0.2009112009763413
            double lpdf = cauchy.LogProbabilityDensityFunction(x: 0.27); // -1.6048922547266871
            double ccdf = cauchy.ComplementaryDistributionFunction(x: 0.27); // 0.53031974158391437
            double icdf = cauchy.InverseDistributionFunction(p: 0.69358638272337991); // 1.5130304686978195

            double hf = cauchy.HazardFunction(x: 0.27); // 0.3788491832800277
            double chf = cauchy.CumulativeHazardFunction(x: 0.27); // 0.63427516833243092

            string str = cauchy.ToString(System.Globalization.CultureInfo.InvariantCulture); // "Cauchy(x; x0 = 0.42, γ = 1.57)

            Assert.IsTrue(Double.IsNaN(mean));
            Assert.IsTrue(Double.IsNaN(var));
            Assert.AreEqual(0.42, median);

            Assert.AreEqual(0.63427516833243092, chf);
            Assert.AreEqual(0.46968025841608563, cdf);
            Assert.AreEqual(0.2009112009763413, pdf);
            Assert.AreEqual(-1.6048922547266871, lpdf);
            Assert.AreEqual(0.3788491832800277, hf);
            Assert.AreEqual(0.53031974158391437, ccdf);
            Assert.AreEqual(1.5130304686978195, icdf);
            Assert.AreEqual("Cauchy(x; x0 = 0.42, γ = 1.57)", str);
        }


        #region Sample
        // 500 samples drawn from a Cauchy distribution with t=4.2, s=0.21
        private static double[] samples =
        {
            4.25847161436748, 4.32879020336098, 4.10200384890093, 4.28335743818132, 4.51142173498328, 0.733112801568828, 3.42155584829012, 6.84721416706609, 4.30812425693285, 3.96366085968445, 4.13430430860427, 3.67928199937881, 
            4.01302162768263, 4.00010572364253, 4.07728464429852, 3.79490565751733, 4.09133867767407, 3.67756964373878, 4.75130809057927, 3.51183717030418, 5.14004955250686, 4.13105132634635, 2.80024939323037, 4.08658237419147, 
            4.39225369299029, 4.47911285010043, 4.22982408889788, 4.33910258763419, 4.80486891269499, 2.99208977730171, 4.05108625620017, 2.76304474229485, 3.90214952580950, 4.37391143494904, 4.37568435951926, 4.71986836241791, 
            4.25563315893476, 3.26991119938066, 5.04819116030443, 4.48975187886194, 4.03278145616367, 4.22898818335310, 8.58748953584592, 4.36897745057363, 4.57907667268651, 4.15531294007488, 4.18056490800589, 4.24054764089851, 
            4.01379222791749, 4.40870887575103, 4.20256502566270, 4.30436252074993, 4.05509935249253, 3.74904615848929, 4.18385291349554, 4.10315609891407, 4.46744221195547, 4.45432726549978, 4.32287245589395, 3.72901149956083, 
            1.10372931706176, 4.23995073679438, 4.04825008807417, 5.28987410617025, 7.69620805544189, 4.03350633463175, 4.49061216413752, 4.82041981667919, 4.26643356703935, 4.75059910313347, 5.37556713381398, 4.23269159729185, 
            4.38325683116069, 4.25164505120715, 1.62055298439914, 4.16438907281681, 4.30394517357711, 4.21400904412459, 4.11093069598928, 5.24944494274226, 4.55390411022982, 4.60923730078642, 4.11110312605242, 4.26329566635635, 
            4.69616000054394, 5.19054694169317, 4.32283000432347, 3.92353328081541, 4.31022023697009, 3.28815976951205, 4.13664063060513, 4.32147665734678, 5.19399492993667, 4.51099821034801, 4.18979794778483, 4.41909967954456, 
            4.14399983845938, 6.56300813754927, 9.75605922451054, 4.66179694334570, 4.12356243071170, 4.16993877908340, 3.98558272812690, 4.46115145040863, 4.74453243050063, 4.95560179681484, 4.23888816573911, 4.26740849723559, 
            3.78423116786436, 4.84433952450633, 4.16700538797175, 3.92153321266539, 4.84391040439706, 4.42729923162452, 4.74274746392787, 4.03170434556401, 4.32708374465603, 4.31915306160044, 3.68300577777266, 4.13706615397199, 
            4.02097021004352, 4.37005800486779, 4.03000110867243, 4.82098189481425, 4.54653552234482, 4.12441510584104, 4.19861647919825, 4.34739855110090, 4.56647923160395, 4.27532879506721, 4.25023317704617, 4.07225569457041,
            4.17107086547742, 4.36694106075752, 4.75262284051666, 4.37468008824896, 0.612739390626911, 4.32848540087473, 4.15891985885018, 4.15844794990708, 3.65482703508856, 4.51899881563083, 4.07118061124924, 3.98496319344102,
            4.08687170594675, 4.11354964503711, 4.23093401564828, 4.24137406075139, 4.12870643519511, 4.13040093453403, 4.21014616788051, 4.31333586909132, 5.55101691901425, 4.37635295319552, 4.13182502408739, 4.55990298176398, 
            3.73231192649851, 3.10784498781461, 3.42517286997062, 3.82885191056715, 4.07060286074569, 4.04915981753684, -1.51998272358839, 4.22646555976451, 3.52021739734418, 3.77645065906194, 4.29177039019145, 4.64381075719312, 
            6.78739706357680, 4.24752168150130, 25.4214120296199, 4.23566017039852, 4.21020650051307, 4.07641294313237, 4.15306068433261, 4.19459310887690, 3.27468552874152, 4.77054998176912, 3.18003716899339, 4.15732574913765,
            4.54665987534050, 4.12775778748501, 4.27820461758125, 4.52780453102671, 4.76233784325252, 5.15514216332050, 3.89262695067268, 4.00102931495938, 4.83186585516040, 4.26342327446325, 4.20253355722507, 4.27770521823986,
            4.52958323886220, 4.22110905218988, 3.91488712088068, 4.16936739686525, 4.15161019112560, 6.16161555033531, 4.28318689949440, 4.34797514253144, 4.37391011087900, 4.09040655105892, 4.21121982336206, 4.23780401114666, 
            3.80784461767131, 4.24146720075553, 4.34739667655573, 4.15059796868166, 4.57160757159019, 4.38677254131826, 4.10120651412959, 4.16958223566509, 4.12169378537305, 4.44674773086234, 4.39020809434470, 4.15325186368118, 
            4.34636437152078, 5.40802890004580, 4.46083165611538, 4.35826321391286, 3.61285229613301, 4.12434366627969, 4.26165843872956, 4.17305519668212, 2.88321650159950, 3.95981261206024, 4.56599945157097, -0.0692588456940610,
            4.66012082352976, 3.36100738793559, 4.32334261267918, 4.20013941793038, 3.94287103607197, 4.24806105127897, 3.68007624256126, 4.32523441827507, 4.26793096910667, 3.01816474906042, 3.02602142423652, 3.79573476852700, 
            0.797514674316809, 4.15663185724776, 4.56081417841126, 4.28115958160782, 4.21329780001524, 4.66072367354259, 3.53741940487131, 4.90665566746333, 3.60509826236582,
        };
        #endregion

    }
}
