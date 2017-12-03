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
    using Accord.IO;
    using Accord.Math;
    using Accord.Statistics;
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Kernels;
    using NUnit.Framework;
    using System.Data;
    using System.IO;

    [TestFixture]
    public class KernelPrincipalComponentAnalysisTest
    {

        // Lindsay's tutorial data
        private static double[,] data =
        {
            { 2.5,  2.4 },
            { 0.5,  0.7 },
            { 2.2,  2.9 },
            { 1.9,  2.2 },
            { 3.1,  3.0 },
            { 2.3,  2.7 },
            { 2.0,  1.6 },
            { 1.0,  1.1 },
            { 1.5,  1.6 },
            { 1.1,  0.9 }
        };


        [Test]
        public void TransformTest()
        {
            // Using a linear kernel should be equivalent to standard PCA
            IKernel kernel = new Linear();

            // Create analysis
            var target = new KernelPrincipalComponentAnalysis(data, kernel, AnalysisMethod.Center);

            // Compute
            target.Compute();

            double[,] actual = target.Transform(data, 2);

            // first inversed.. ?
            double[,] expected = new double[,]
            {
                { -0.827970186,  0.175115307 },
                {  1.77758033,  -0.142857227 },
                { -0.992197494, -0.384374989 },
                { -0.274210416, -0.130417207 },
                { -1.67580142,   0.209498461 },
                { -0.912949103, -0.175282444 },
                {  0.099109437,  0.349824698 },
                {  1.14457216,  -0.046417258 },
                {  0.438046137, -0.017764629 },
                {  1.22382056,   0.162675287 },
            };

            // Verify both are equal with 0.001 tolerance value
            Assert.IsTrue(Matrix.IsEqual(actual, expected, 0.0001));

            // Assert the result equals the transformation of the input
            double[,] result = target.Result;
            double[,] projection = target.Transform(data);
            Assert.IsTrue(Matrix.IsEqual(result, projection, 0.000001));

            Assert.AreEqual(2, target.Eigenvalues.Length);
            Assert.AreEqual(10, target.ComponentMatrix.GetLength(0));
            Assert.AreEqual(2, target.ComponentMatrix.GetLength(1));
        }

        [Test]
        public void TransformTest2()
        {
            // Using a linear kernel should be equivalent to standard PCA
            IKernel kernel = new Linear();

            // Create analysis
            var target = new KernelPrincipalComponentAnalysis(data, kernel, AnalysisMethod.Center);

            // Set the minimum variance threshold to 0.001
            target.Threshold = 0.001;

            // Compute
            target.Compute();

            var r = target.Result;

            double[,] actual = target.Transform(data, 1);

            // first inversed.. ?
            double[,] expected = new double[,]
            {
                { -0.827970186 },
                {  1.77758033  },
                { -0.992197494 },
                { -0.274210416 },
                { -1.67580142  },
                { -0.912949103 },
                {  0.099109437 },
                {  1.14457216  },
                {  0.438046137 },
                {  1.22382056  },
            };

            // Verify both are equal with 0.001 tolerance value
            Assert.IsTrue(Matrix.IsEqual(actual, expected, 0.0001));

            // Assert the result equals the transformation of the input
            double[,] result = target.Result;
            double[,] projection = target.Transform(data);
            Assert.IsTrue(Matrix.IsEqual(result, projection, 0.000001));
        }

        [Test]
        public void TransformTest3()
        {
            // Using a linear kernel should be equivalent to standard PCA
            IKernel kernel = new Linear();

            // Create analysis
            KernelPrincipalComponentAnalysis target = new KernelPrincipalComponentAnalysis(data, kernel);

            // Compute
            target.Compute();

            bool thrown = false;
            try
            {
                double[,] actual = target.Transform(data, 11);
            }
            catch
            {
                thrown = true;
            }

            Assert.IsTrue(thrown);
        }

        [Test]
        public void TransformTest4()
        {
            // Tested against R's kernlab

            // test <-   c(16, 117, 94, 132, 13, 73, 68, 129, 91, 50, 56, 12, 145, 105, 35, 53, 38, 51, 85, 116)
            int[] test = { 15, 116, 93, 131, 12, 72, 67, 128, 90, 49, 55, 11, 144, 104, 34, 52, 37, 50, 84, 115 };

            // data(iris)
            double[,] iris = create_iris();


            // kpc <- kpca(~.,data=iris[-test,-5],kernel="rbfdot",kpar=list(sigma=0.2),features=2)
            var data = iris.Remove(test, new[] { 4 });
            var kernel = new Gaussian() { Gamma = 1 };
            var kpc = new KernelPrincipalComponentAnalysis(data, kernel);

            kpc.Compute(2);

            var rotated = kpc.Result;
            var pcv = kpc.ComponentMatrix;
            var eig = kpc.Eigenvalues;

            double[] expected_eig = { 28.542404060412132, 15.235596653653861 };
            double[,] expected_pcv = expected_r();

            Assert.IsTrue(Matrix.IsEqual(expected_eig, eig, 1e-10));
            Assert.IsTrue(Matrix.IsEqual(expected_pcv, pcv, 1e-10));

            double[,] irisSubset = iris_sub();

            var testing = iris.Submatrix(test, new[] { 0, 1, 2, 3 });

            Assert.IsTrue(Matrix.IsEqual(irisSubset, testing));


            double[,] proj = kpc.Transform(testing);

            double[,] expectedProjection = expected_p();

            Assert.IsTrue(expectedProjection.IsEqual(proj, 1e-6));
        }

        [Test]
        public void test_kernlab_new_method()
        {
            // Tested against R's kernlab

            // test <-   c(16, 117, 94, 132, 13, 73, 68, 129, 91, 50, 56, 12, 145, 105, 35, 53, 38, 51, 85, 116)
            int[] test = { 15, 116, 93, 131, 12, 72, 67, 128, 90, 49, 55, 11, 144, 104, 34, 52, 37, 50, 84, 115 };

            // data(iris)
            double[,] iris = create_iris();


            // kpc <- kpca(~.,data=iris[-test,-5],kernel="rbfdot",kpar=list(sigma=0.2),features=2)
            var data = iris.Remove(test, new[] { 4 });
            var kernel = new Gaussian() { Gamma = 1 };
            var kpc = new KernelPrincipalComponentAnalysis(kernel);

            kpc.NumberOfOutputs = 2;
            var transform = kpc.Learn(data);

            var rotated = kpc.Result;
            var pcv = kpc.ComponentMatrix;
            var eig = kpc.Eigenvalues;

            double[] expected_eig = { 28.542404060412132, 15.235596653653861 };
            double[,] expected_pcv = expected_r();

            Assert.IsTrue(Matrix.IsEqual(expected_eig, eig, 1e-10));
            Assert.IsTrue(Matrix.IsEqual(expected_pcv, pcv, 1e-10));

            double[,] irisSubset = iris_sub();

            var testing = iris.Submatrix(test, new[] { 0, 1, 2, 3 });

            Assert.IsTrue(Matrix.IsEqual(irisSubset, testing));

            double[,] expectedProjection = expected_p();

            double[,] proj = kpc.Transform(testing);
            Assert.IsTrue(expectedProjection.IsEqual(proj, 1e-6));

            double[][] proj2 = kpc.Transform(testing.ToJagged());
            Assert.IsTrue(expectedProjection.IsEqual(proj, 1e-6));

            double[][] proj3 = transform.Transform(testing.ToJagged());
            Assert.IsTrue(expectedProjection.IsEqual(proj, 1e-6));
        }

        private static double[,] expected_p()
        {
            double[,] expectedProjection =
            {
                #region expected projection without R's /m normalization
                { -0.262045246354652, 0.00522592803297944 },
                { 0.379100878015288, 0.588655883281293 },
                { 0.071459976634984, -0.256289120689712 },
                { 0.0208479105625222, 0.139976227370984 },
                { -0.634360697174036, -0.0253624940325211 },
                { 0.467841054198416, 0.0142078237631541 },
                { 0.3464723948387, -0.62695357333265 },
                { 0.327328144102457, 0.603762286061834 },
                { 0.351964514241981, -0.539035845068089 },
                { -0.759054821003877, -0.035920361137046 },
                { 0.449638018323254, -0.492049890038061 },
                { -0.732335083049923, -0.0341252836840602 },
                { 0.192183096200302, 0.580343336854431 },
                { 0.256170478557119, 0.639157216957949 },
                { -0.703212303846621, -0.0317868463626801 },
                { 0.3515430820112, 0.224868844202495 },
                { -0.722813976459246, -0.0325608519534802 },
                { 0.286990265042346, 0.102161459040097 },
                { 0.354904620698745, -0.390810675482863 },
                { 0.332125880099634, 0.566660263312128 } 
                #endregion
            };
            return expectedProjection;
        }

        private static double[,] iris_sub()
        {
            double[,] irisSubset =
            {
                #region Iris subset for testing
                { 5.7,  4.4,  1.5,  0.4 },
                { 6.5,  3.0,  5.5,  1.8 },
                { 5.0,  2.3,  3.3,  1.0 },
                { 7.9,  3.8,  6.4,  2.0 },
                { 4.8,  3.0,  1.4,  0.1 },
                { 6.3,  2.5,  4.9,  1.5 },
                { 5.8,  2.7,  4.1,  1.0 },
                { 6.4,  2.8,  5.6,  2.1 },
                { 5.5,  2.6,  4.4,  1.2 },
                { 5.0,  3.3,  1.4,  0.2 },
                { 5.7,  2.8,  4.5,  1.3 },
                { 4.8,  3.4,  1.6,  0.2 },
                { 6.7,  3.3,  5.7,  2.5 },
                { 6.5,  3.0,  5.8,  2.2 },
                { 4.9,  3.1,  1.5,  0.2 },
                { 6.9,  3.1,  4.9,  1.5 },
                { 4.9,  3.6,  1.4,  0.1 },
                { 7.0,  3.2,  4.7,  1.4 },
                { 5.4,  3.0,  4.5,  1.5 },
                { 6.4,  3.2,  5.3,  2.3 },
                #endregion
            };
            return irisSubset;
        }

        private static double[,] expected_r()
        {
            double[,] expected_pcv =
            {
                #region expected PCV without R's / m normalization
                { -0.0266876243479222, -0.00236424647855596 },
                { -0.0230827502249994, -0.00182207284533632 },
                { -0.0235846044938792, -0.00184417084258023 },
                { -0.0219741114149703, -0.00162806197434679 },
                { -0.0262369254935451, -0.00228351232176506 },
                { -0.0194129527129315, -0.00128157547584046 },
                { -0.0233710173690426, -0.00183018780267092 },
                { -0.0270621345426091, -0.00244551460941156 },
                { -0.01660360115437, -0.000759995006404066 },
                { -0.0241543595644871, -0.00200028851593623 },
                { -0.0229396684027426, -0.00178791975184668 },
                { -0.0141003945759371, -0.000349601250510858 },
                { -0.0122801944616023, -0.00011745003303124 },
                { -0.0195599909514198, -0.00123924882430174 },
                { -0.0267285199984888, -0.00237835151986576 },
                { -0.0164051441608544, -0.000826433392421186 },
                { -0.0241385103747907, -0.00196921837902471 },
                { -0.0228276819006861, -0.00190280719845395 },
                { -0.0250090125071634, -0.00212322482498387 },
                { -0.018268574949505, -0.00103729989327242 },
                { -0.0239555047501124, -0.00216590896337712 },
                { -0.0218837974825259, -0.00180921340210779 },
                { -0.0228699114274226, -0.00189079843025579 },
                { -0.0262414571955617, -0.00238666692022459 },
                { -0.02628286882499, -0.00232756740052467 },
                { -0.0261369413490628, -0.00229661111040973 },
                { -0.0237893959503383, -0.00195315162891338 },
                { -0.0237354902927562, -0.00197089686864334 },
                { -0.0234996712936547, -0.0019545398678434 },
                { -0.0179796342021205, -0.00100004281923827 },
                { -0.0143171193045046, -0.000421228427584423 },
                { -0.0241702773143143, -0.00196078350204665 },
                { -0.0215781675204649, -0.00158425565875557 },
                { -0.0174405137049866, -0.000872162100068597 },
                { -0.0268982927662575, -0.00242925852652081 },
                { -0.0261930727206913, -0.00227548913089953 },
                { -0.00807266421494459, 0.000505384268295841 },
                { -0.0189832707329651, -0.00111618515851182 },
                { -0.023789443724428, -0.00203239968623136 },
                { -0.0201457716377357, -0.00150731437393246 },
                { -0.0226387870826046, -0.00174799717649726 },
                { -0.0237772220904885, -0.00192536309172948 },
                { -0.0227864577886965, -0.00172757669197999 },
                { -0.0241368046325238, -0.00197147776349598 },
                { 0.0162307596401467, -0.00932217153629181 },
                { 0.00924104683890504, -0.0371256298132236 },
                { 0.0172460604733757, -0.00601678602419225 },
                { 0.0164784470762724, -0.00012129053123478 },
                { 0.00225808467595593, -0.0155701555363185 },
                { 0.0152659569368524, -0.00695503994803249 },
                { 0.00795619200816849, -0.034188555904496 },
                { 0.00255986394744671, -0.0156335839305463 },
                { 0.0157735235376026, -0.0339711483141172 },
                { 0.00860192955815661, -0.0310332456913026 },
                { 0.0188286198627367, -0.0143146603067418 },
                { 0.0081385823965042, -0.0358483794263587 },
                { 0.0131727085950618, -0.00748671161967017 },
                { 0.0150373592446138, -0.0269773780381651 },
                { 0.0126779242124717, -0.0162727482334416 },
                { 0.00983265072294127, -0.0416039968698012 },
                { 0.0162669562079483, -0.000151449934923387 },
                { 0.0137854766363786, -0.0375070307423622 },
                { 0.0170058660389757, -0.0184237621007135 },
                { 0.0154946725649067, -0.0227889410670457 },
                { 0.014708096275464, -0.011169199019916 },
                { 0.0135541309647514, 0.00627293040317239 },
                { 0.0153982178833786, 0.0228745884070871 },
                { 0.0186116855914761, -0.0238281923214434 },
                { 0.00661605660296714, -0.0332168101819555 },
                { 0.00812230548276198, -0.0380947707628449 },
                { 0.00704157480127114, -0.0353293378234606 },
                { 0.0118813500247593, -0.0433955442329169 },
                { 0.0168403649935284, 0.00717417511929008 },
                { 0.0144885311444922, -0.0128879186387195 },
                { 0.0148385454088314, 0.00481616750741218 },
                { 0.0127847825706042, -0.0211295878510692 },
                { 0.0126141523297424, -0.0394948238730571 },
                { 0.0105804278587419, -0.0411832808826231 },
                { 0.0185081272399827, -0.0181339486962481 },
                { 0.0124993892884636, -0.0434407731971394 },
                { 0.0135227934497893, -0.0415894662412569 },
                { 0.0136028421755366, -0.0388446289823116 },
                { 0.0144604273990706, -0.0404041262573942 },
                { 0.0165646866155949, -0.0294021220435322 },
                { 0.00146858312783178, -0.0134333124454357 },
                { 0.0137785343752508, -0.0429733697468562 },
                { 0.00510997410924024, 0.0292833047881736 },
                { 0.014720812085274, 0.00944264118212137 },
                { 0.00598583015620509, 0.038742545754176 },
                { 0.0125544895333347, 0.0349170237345097 },
                { 0.00140911493699792, 0.0164126558963803 },
                { 0.00546764790022381, -0.0140904440836446 },
                { 0.0029496609416271, 0.0249945804373137 },
                { 0.00769932014045035, 0.0313261912102264 },
                { 0.00266139821119332, 0.0231665860038695 },
                { 0.0147368620502789, 0.0315131740192214 },
                { 0.0149582869669828, 0.0314622232024109 },
                { 0.0106818628524054, 0.0443273862959601 },
                { 0.0123400540017047, 0.00422397833506881 },
                { 0.0101542521522688, 0.0157643916651046 },
                { 0.000660568495239385, 0.0087957765410289 },
                { 0.000634971613911479, 0.00896839373841372 },
                { 0.0119909422310846, -0.0019235494568038 },
                { 0.00742254354227651, 0.0421145349479265 },
                { 0.0130658707704511, 0.000658712215109605 },
                { 0.00103199141821948, 0.0130131637684367 },
                { 0.0180388007633923, 0.0112135357385706 },
                { 0.00879897568153878, 0.0428371609763469 },
                { 0.00466754803065601, 0.0321456973019424 },
                { 0.0188135431637204, 0.00458127473828957 },
                { 0.0184728744733845, 0.00843677964296344 },
                { 0.0055676853191067, 0.0305087649038716 },
                { 0.0033635667326866, 0.026834775073324 },
                { 0.0108405706484462, 0.0394739066547236 },
                { 0.0172770225385115, 0.0124967454210229 },
                { 0.0100507351970463, 0.0166565450918105 },
                { 0.00209404741665691, 0.0205532162586405 },
                { 0.0078782378323636, 0.0341148825697675 },
                { 0.0132731813046484, 0.0368540207320806 },
                { 0.0182550250587539, 0.000797957664175355 },
                { 0.0102561686092287, 0.0420705939254378 },
                { 0.00857331992305152, 0.0423810139397453 },
                { 0.00964648674506066, 0.0337591223497657 },
                { 0.014720812085274, 0.00944264118212137 },
                { 0.00659947194015947, 0.0404655648392282 },
                { 0.011337029514041, 0.0378339231578959 },
                { 0.0154602034267052, 0.0153085911335171 },
                { 0.0152371977428677, 0.0355309408870963 },
                { 0.0096520854263212, 0.0316677099444034 },
                { 0.016280981143395, 0.011860068380509 } 
#endregion
            };
            return expected_pcv;
        }

        private static double[,] create_iris()
        {
            double[,] iris =
            {
                #region Fisher's iris dataset
                { 5.1, 3.5,  1.4, 0.2, 0 },
                { 4.9, 3.0,  1.4, 0.2, 0 },
                { 4.7, 3.2,  1.3, 0.2, 0 },
                { 4.6, 3.1,  1.5, 0.2, 0 },
                { 5.0, 3.6,  1.4, 0.2, 0 },
                { 5.4, 3.9,  1.7, 0.4, 0 },
                { 4.6, 3.4,  1.4, 0.3, 0 },
                { 5.0, 3.4,  1.5, 0.2, 0 },
                { 4.4, 2.9,  1.4, 0.2, 0 },
                { 4.9, 3.1,  1.5, 0.1, 0 },
                { 5.4, 3.7,  1.5, 0.2, 0 },
                { 4.8, 3.4,  1.6, 0.2, 0 },
                { 4.8, 3.0,  1.4, 0.1, 0 },
                { 4.3, 3.0,  1.1, 0.1, 0 },
                { 5.8, 4.0,  1.2, 0.2, 0 },
                { 5.7, 4.4,  1.5, 0.4, 0 },
                { 5.4, 3.9,  1.3, 0.4, 0 },
                { 5.1, 3.5,  1.4, 0.3, 0 },
                { 5.7, 3.8,  1.7, 0.3, 0 },
                { 5.1, 3.8,  1.5, 0.3, 0 },
                { 5.4, 3.4,  1.7, 0.2, 0 },
                { 5.1, 3.7,  1.5, 0.4, 0 },
                { 4.6, 3.6,  1.0, 0.2, 0 },
                { 5.1, 3.3,  1.7, 0.5, 0 },
                { 4.8, 3.4,  1.9, 0.2, 0 },
                { 5.0, 3.0,  1.6, 0.2, 0 },
                { 5.0, 3.4,  1.6, 0.4, 0 },
                { 5.2, 3.5,  1.5, 0.2, 0 },
                { 5.2, 3.4,  1.4, 0.2, 0 },
                { 4.7, 3.2,  1.6, 0.2, 0 },
                { 4.8, 3.1,  1.6, 0.2, 0 },
                { 5.4, 3.4,  1.5, 0.4, 0 },
                { 5.2, 4.1,  1.5, 0.1, 0 },
                { 5.5, 4.2,  1.4, 0.2, 0 },
                { 4.9, 3.1,  1.5, 0.2, 0 },
                { 5.0, 3.2,  1.2, 0.2, 0 },
                { 5.5, 3.5,  1.3, 0.2, 0 },
                { 4.9, 3.6,  1.4, 0.1, 0 },
                { 4.4, 3.0,  1.3, 0.2, 0 },
                { 5.1, 3.4,  1.5, 0.2, 0 },
                { 5.0, 3.5,  1.3, 0.3, 0 },
                { 4.5, 2.3,  1.3, 0.3, 0 },
                { 4.4, 3.2,  1.3, 0.2, 0 },
                { 5.0, 3.5,  1.6, 0.6, 0 },
                { 5.1, 3.8,  1.9, 0.4, 0 },
                { 4.8, 3.0,  1.4, 0.3, 0 },
                { 5.1, 3.8,  1.6, 0.2, 0 },
                { 4.6, 3.2,  1.4, 0.2, 0 },
                { 5.3, 3.7,  1.5, 0.2, 0 },
                { 5.0, 3.3,  1.4, 0.2, 0 },
                { 7.0, 3.2,  4.7, 1.4, 1 },
                { 6.4, 3.2,  4.5, 1.5, 1 },
                { 6.9, 3.1,  4.9, 1.5, 1 },
                { 5.5, 2.3,  4.0, 1.3, 1 },
                { 6.5, 2.8,  4.6, 1.5, 1 },
                { 5.7, 2.8,  4.5, 1.3, 1 },
                { 6.3, 3.3,  4.7, 1.6, 1 },
                { 4.9, 2.4,  3.3, 1.0, 1 },
                { 6.6, 2.9,  4.6, 1.3, 1 },
                { 5.2, 2.7,  3.9, 1.4, 1 },
                { 5.0, 2.0,  3.5, 1.0, 1 },
                { 5.9, 3.0,  4.2, 1.5, 1 },
                { 6.0, 2.2,  4.0, 1.0, 1 },
                { 6.1, 2.9,  4.7, 1.4, 1 },
                { 5.6, 2.9,  3.6, 1.3, 1 },
                { 6.7, 3.1,  4.4, 1.4, 1 },
                { 5.6, 3.0,  4.5, 1.5, 1 },
                { 5.8, 2.7,  4.1, 1.0, 1 },
                { 6.2, 2.2,  4.5, 1.5, 1 },
                { 5.6, 2.5,  3.9, 1.1, 1 },
                { 5.9, 3.2,  4.8, 1.8, 1 },
                { 6.1, 2.8,  4.0, 1.3, 1 },
                { 6.3, 2.5,  4.9, 1.5, 1 },
                { 6.1, 2.8,  4.7, 1.2, 1 },
                { 6.4, 2.9,  4.3, 1.3, 1 },
                { 6.6, 3.0,  4.4, 1.4, 1 },
                { 6.8, 2.8,  4.8, 1.4, 1 },
                { 6.7, 3.0,  5.0, 1.7, 1 },
                { 6.0, 2.9,  4.5, 1.5, 1 },
                { 5.7, 2.6,  3.5, 1.0, 1 },
                { 5.5, 2.4,  3.8, 1.1, 1 },
                { 5.5, 2.4,  3.7, 1.0, 1 },
                { 5.8, 2.7,  3.9, 1.2, 1 },
                { 6.0, 2.7,  5.1, 1.6, 1 },
                { 5.4, 3.0,  4.5, 1.5, 1 },
                { 6.0, 3.4,  4.5, 1.6, 1 },
                { 6.7, 3.1,  4.7, 1.5, 1 },
                { 6.3, 2.3,  4.4, 1.3, 1 },
                { 5.6, 3.0,  4.1, 1.3, 1 },
                { 5.5, 2.5,  4.0, 1.3, 1 },
                { 5.5, 2.6,  4.4, 1.2, 1 },
                { 6.1, 3.0,  4.6, 1.4, 1 },
                { 5.8, 2.6,  4.0, 1.2, 1 },
                { 5.0, 2.3,  3.3, 1.0, 1 },
                { 5.6, 2.7,  4.2, 1.3, 1 },
                { 5.7, 3.0,  4.2, 1.2, 1 },
                { 5.7, 2.9,  4.2, 1.3, 1 },
                { 6.2, 2.9,  4.3, 1.3, 1 },
                { 5.1, 2.5,  3.0, 1.1, 1 },
                { 5.7, 2.8,  4.1, 1.3, 1 },
                { 6.3, 3.3,  6.0, 2.5, 2 },
                { 5.8, 2.7,  5.1, 1.9, 2 },
                { 7.1, 3.0,  5.9, 2.1, 2 },
                { 6.3, 2.9,  5.6, 1.8, 2 },
                { 6.5, 3.0,  5.8, 2.2, 2 },
                { 7.6, 3.0,  6.6, 2.1, 2 },
                { 4.9, 2.5,  4.5, 1.7, 2 },
                { 7.3, 2.9,  6.3, 1.8, 2 },
                { 6.7, 2.5,  5.8, 1.8, 2 },
                { 7.2, 3.6,  6.1, 2.5, 2 },
                { 6.5, 3.2,  5.1, 2.0, 2 },
                { 6.4, 2.7,  5.3, 1.9, 2 },
                { 6.8, 3.0,  5.5, 2.1, 2 },
                { 5.7, 2.5,  5.0, 2.0, 2 },
                { 5.8, 2.8,  5.1, 2.4, 2 },
                { 6.4, 3.2,  5.3, 2.3, 2 },
                { 6.5, 3.0,  5.5, 1.8, 2 },
                { 7.7, 3.8,  6.7, 2.2, 2 },
                { 7.7, 2.6,  6.9, 2.3, 2 },
                { 6.0, 2.2,  5.0, 1.5, 2 },
                { 6.9, 3.2,  5.7, 2.3, 2 },
                { 5.6, 2.8,  4.9, 2.0, 2 },
                { 7.7, 2.8,  6.7, 2.0, 2 },
                { 6.3, 2.7,  4.9, 1.8, 2 },
                { 6.7, 3.3,  5.7, 2.1, 2 },
                { 7.2, 3.2,  6.0, 1.8, 2 },
                { 6.2, 2.8,  4.8, 1.8, 2 },
                { 6.1, 3.0,  4.9, 1.8, 2 },
                { 6.4, 2.8,  5.6, 2.1, 2 },
                { 7.2, 3.0,  5.8, 1.6, 2 },
                { 7.4, 2.8,  6.1, 1.9, 2 },
                { 7.9, 3.8,  6.4, 2.0, 2 },
                { 6.4, 2.8,  5.6, 2.2, 2 },
                { 6.3, 2.8,  5.1, 1.5, 2 },
                { 6.1, 2.6,  5.6, 1.4, 2 },
                { 7.7, 3.0,  6.1, 2.3, 2 },
                { 6.3, 3.4,  5.6, 2.4, 2 },
                { 6.4, 3.1,  5.5, 1.8, 2 },
                { 6.0, 3.0,  4.8, 1.8, 2 },
                { 6.9, 3.1,  5.4, 2.1, 2 },
                { 6.7, 3.1,  5.6, 2.4, 2 },
                { 6.9, 3.1,  5.1, 2.3, 2 },
                { 5.8, 2.7,  5.1, 1.9, 2 },
                { 6.8, 3.2,  5.9, 2.3, 2 },
                { 6.7, 3.3,  5.7, 2.5, 2 },
                { 6.7, 3.0,  5.2, 2.3, 2 },
                { 6.3, 2.5,  5.0, 1.9, 2 },
                { 6.5, 3.0,  5.2, 2.0, 2 },
                { 6.2, 3.4,  5.4, 2.3, 2 },
                { 5.9, 3.0,  5.1, 1.8, 2 },
                #endregion
            };
            return iris;
        }

        [Test]
        public void TransformTest5()
        {
            int element = 10;
            int dimension = 20;

            double[,] data = new double[element, dimension];

            int x = 0;

            for (int i = 0; i < element; i++)
            {
                for (int j = 0; j < dimension; j++)
                    data[i, j] = x;

                x += 10;
            }

            IKernel kernel = new Gaussian(10.0);
            var kpca = new KernelPrincipalComponentAnalysis(data, kernel, AnalysisMethod.Center);

            kpca.Compute();

            double[,] result = kpca.Transform(data, 2);

            double[,] expected = new double[,]
            {
                { -0.23053882357602, -0.284413654763538 },
                { -0.387883199575312, -0.331485820285834 },
                { -0.422077400361521, -0.11134948984113 },
                { -0.322265008788599, 0.23632015508648 },
                { -0.12013575394419, 0.490928809797139 },
                { 0.120135753938394, 0.490928809796094 },
                { 0.322265008787236, 0.236320155085067 },
                { 0.422077400363969, -0.111349489837512 },
                { 0.38788319957867, -0.331485820278937 },
                { 0.230538823577373, -0.28441365475783 }
            };

            Assert.IsTrue(result.IsEqual(expected, 1e-10));
        }

        [Test]
        public void TransformTest_Jagged()
        {
            double[][] sourceMatrix = new double[][]
            {
                new double[] { 2.5,  2.4 },
                new double[] { 0.5,  0.7 },
                new double[] { 2.2,  2.9 },
                new double[] { 1.9,  2.2 },
                new double[] { 3.1,  3.0 },
                new double[] { 2.3,  2.7 },
                new double[] { 2.0,  1.6 },
                new double[] { 1.0,  1.1 },
                new double[] { 1.5,  1.6 },
                new double[] { 1.1,  0.9 }
            };

            // Create a new linear kernel
            IKernel kernel = new Linear();

            // Creates the Kernel Principal Component Analysis of the given data
            var kpca = new KernelPrincipalComponentAnalysis(sourceMatrix, kernel);

            // Compute the Kernel Principal Component Analysis
            kpca.Compute();

            double[] actual1 = kpca.Transform(sourceMatrix[0]);

            double[][] actual = kpca.Transform(sourceMatrix);

            double[][] expected =
            {
                new double[] { -0.827970186,  0.175115307 },
                new double[] {  1.77758033,  -0.142857227 },
                new double[] { -0.992197494, -0.384374989 },
                new double[] { -0.274210416, -0.130417207 },
                new double[] { -1.67580142,   0.209498461 },
                new double[] { -0.912949103, -0.175282444 },
                new double[] {  0.099109437,  0.349824698 },
                new double[] {  1.14457216,  -0.046417258 },
                new double[] {  0.438046137, -0.017764629 },
                new double[] {  1.22382056,   0.162675287 },
            };

            Assert.IsTrue(Matrix.IsEqual(expected[0], actual1, 0.0001));

            // Verify both are equal with 0.001 tolerance value
            Assert.IsTrue(Matrix.IsEqual(actual, expected, 0.0001));

            // Assert the result equals the transformation of the input
            double[,] result = kpca.Result;
            double[,] projection = kpca.Transform(data);
            Assert.IsTrue(Matrix.IsEqual(result, projection, 0.000001));
        }

        [Test]
        public void RevertTest()
        {
            // Using a linear kernel should be equivalent to standard PCA
            IKernel kernel = new Linear();

            // Create analysis
            var target = new KernelPrincipalComponentAnalysis(data, kernel, AnalysisMethod.Center);

            // Compute
            target.Compute();

            // Compute image
            double[,] image = target.Transform(data, 2);

            // Compute pre-image
            double[,] preimage = target.Revert(image);

            // Check if pre-image equals the original data
            Assert.IsTrue(Matrix.IsEqual(data, preimage, 0.0001));
        }

#if !NO_EXCEL
        [Test]
        [Category("Office")]
        public void RevertTest2()
        {
            string path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "examples.xls");

            // Create a new reader, opening a given path
            ExcelReader reader = new ExcelReader(path);

            // Afterwards, we can query the file for all
            // worksheets within the specified workbook:
            string[] sheets = reader.GetWorksheetList();

            // Finally, we can request an specific sheet:
            DataTable table = reader.GetWorksheet("Wikipedia");

            // Now, we have loaded the Excel file into a DataTable. We
            // can go further and transform it into a matrix to start
            // running other algorithms on it: 

            double[,] matrix = table.ToMatrix();

            IKernel kernel = new Gaussian(5);

            // Create analysis
            var target = new KernelPrincipalComponentAnalysis(matrix,
                kernel, AnalysisMethod.Center, centerInFeatureSpace: false);

            target.Compute();

            double[,] forward = target.Result;

            double[,] reversion = target.Revert(forward);

            Assert.IsTrue(!reversion.HasNaN());
        }

        [Test]
        [Category("Office")]
        public void RevertTest2_new_method()
        {
            string path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "examples.xls");

            // Create a new reader, opening a given path
            ExcelReader reader = new ExcelReader(path);

            // Afterwards, we can query the file for all
            // worksheets within the specified workbook:
            string[] sheets = reader.GetWorksheetList();

            // Finally, we can request an specific sheet:
            DataTable table = reader.GetWorksheet("Wikipedia");

            // Now, we have loaded the Excel file into a DataTable. We
            // can go further and transform it into a matrix to start
            // running other algorithms on it: 

            double[][] matrix = table.ToArray();

            IKernel kernel = new Gaussian(5);

            // Create analysis
            var target = new KernelPrincipalComponentAnalysis(kernel)
            {
                Method = PrincipalComponentMethod.Center,
                Center = true // Center in feature space
            };

            var regression = target.Learn(matrix);

            double[][] forward = regression.Transform(matrix);

            double[][] reversion = target.Revert(forward);

            Assert.IsTrue(!reversion.HasNaN());
        }

        [Test]
        [Category("Office")]
        public void RevertTest3()
        {
            string path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "examples.xls");

            // Create a new reader, opening a given path
            ExcelReader reader = new ExcelReader(path);

            // Afterwards, we can query the file for all
            // worksheets within the specified workbook:
            string[] sheets = reader.GetWorksheetList();

            // Finally, we can request an specific sheet:
            DataTable table = reader.GetWorksheet("Wikipedia");

            // Now, we have loaded the Excel file into a DataTable. We
            // can go further and transform it into a matrix to start
            // running other algorithms on it: 

            double[,] matrix = table.ToMatrix();

            IKernel kernel = new Polynomial(2);

            // Create analysis
            var target = new KernelPrincipalComponentAnalysis(matrix,
                kernel, AnalysisMethod.Center, centerInFeatureSpace: true);

            target.Compute();

            double[,] forward = target.Result;

            double[,] reversion = target.Revert(forward);

            Assert.IsTrue(!reversion.HasNaN());
        }
#endif

        [Test]
        public void transform_more_columns_than_samples_new_interface()
        {
            // Lindsay's tutorial data
            double[,] datat = data.Transpose();

            var target = new KernelPrincipalComponentAnalysis(new Linear());

            // Compute
            target.Learn(datat);

            // Transform
            double[,] actual = target.Transform(datat);

            // Assert the scores equals the transformation of the input
            double[,] result = target.Result;

            double[,] expected = new double[,]
            {
                {  0.50497524691810358 },
                { -0.504975246918104  }
            }.Multiply(-1);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.01));
            Assert.IsTrue(Matrix.IsEqual(result, actual, 0.01));
        }

        [Test]
        public void kernel_matrix_success()
        {
            var actual = new KernelPrincipalComponentAnalysis(new Linear(), PrincipalComponentMethod.KernelMatrix);
            var expected = new KernelPrincipalComponentAnalysis(data, new Linear(), AnalysisMethod.Standardize);

            Linear kernel = new Linear();
            double[][] K = kernel.ToJagged(data.ZScores().ToJagged());

            // Compute
            actual.Learn(K);
            expected.Compute();

            // Transform
            double[][] actualTransform = actual.Transform(K);
            double[][] expectedTransform1 = expected.Transform(data).ToJagged();
            double[][] expectedTransform2 = expected.Transform(data.ToJagged());


            // Verify both are equal with 0.01 tolerance value
            Assert.IsTrue(Matrix.IsEqual(actualTransform, expectedTransform1, 0.01));
            Assert.IsTrue(Matrix.IsEqual(actualTransform, expectedTransform2, 0.01));
        }


        [Test]
        public void learn_success()
        {
            #region doc_learn_1
            // Reproducing Lindsay Smith's "Tutorial on Principal Component Analysis"
            // using the framework's default method. The tutorial can be found online
            // at http://www.sccg.sk/~haladova/principal_components.pdf

            // Step 1. Get some data
            // ---------------------

            double[][] data =
            {
                new double[] { 2.5,  2.4 },
                new double[] { 0.5,  0.7 },
                new double[] { 2.2,  2.9 },
                new double[] { 1.9,  2.2 },
                new double[] { 3.1,  3.0 },
                new double[] { 2.3,  2.7 },
                new double[] { 2.0,  1.6 },
                new double[] { 1.0,  1.1 },
                new double[] { 1.5,  1.6 },
                new double[] { 1.1,  0.9 }
            };


            // Step 2. Subtract the mean
            // -------------------------
            //   Note: The framework does this automatically. By default, the framework
            //   uses the "Center" method, which only subtracts the mean. However, it is
            //   also possible to remove the mean *and* divide by the standard deviation
            //   (thus performing the correlation method) by specifying "Standardize"
            //   instead of "Center" as the AnalysisMethod.

            var method = PrincipalComponentMethod.Center; // PrincipalComponentMethod.Standardize


            // Step 3. Compute the covariance matrix
            // -------------------------------------
            //   Note: Accord.NET does not need to compute the covariance
            //   matrix in order to compute PCA. The framework uses the SVD
            //   method which is more numerically stable, but may require
            //   more processing or memory. In order to replicate the tutorial
            //   using covariance matrices, please see the next unit test.

            // Create the analysis using the selected method
            var pca = new KernelPrincipalComponentAnalysis(new Linear(), method);

            // Compute it
            pca.Learn(data);


            // Step 4. Compute the eigenvectors and eigenvalues of the covariance matrix
            // -------------------------------------------------------------------------
            //   Note: Since Accord.NET uses the SVD method rather than the Eigendecomposition
            //   method, the Eigenvalues are computed from the singular values. However, it is
            //   not the Eigenvalues themselves which are important, but rather their proportion:

            // Those are the expected eigenvalues, in descending order:
            double[] eigenvalues = { 1.28402771, 0.0490833989 };

            // And this will be their proportion:
            double[] proportion = eigenvalues.Divide(eigenvalues.Sum());


            Assert.IsTrue(proportion.IsEqual(pca.ComponentProportions, rtol: 1e-9));
            Assert.IsTrue(eigenvalues.IsEqual(pca.Eigenvalues.Divide(data.GetLength(0) - 1), rtol: 1e-5));

            // Step 5. Deriving the new data set
            // ---------------------------------

            double[][] actual = pca.Transform(data);

            // transformedData shown in pg. 18
            double[,] expected = new double[,]
            {
                {  0.827970186, -0.175115307 },
                { -1.77758033,   0.142857227 },
                {  0.992197494,  0.384374989 },
                {  0.274210416,  0.130417207 },
                {  1.67580142,  -0.209498461 },
                {  0.912949103,  0.175282444 },
                { -0.099109437, -0.349824698 },
                { -1.14457216,   0.046417258 },
                { -0.438046137,  0.017764629 },
                { -1.22382056,  -0.162675287 },
            }.Multiply(-1);

            // Everything is correct (up to 8 decimal places)
            Assert.IsTrue(expected.IsEqual(actual, atol: 1e-8));

            // Finally, we can project all the data
            double[][] output1 = pca.Transform(data);

            // Or just its first components by setting 
            // NumberOfOutputs to the desired components:
            pca.NumberOfOutputs = 1;

            // And then calling transform again:
            double[][] output2 = pca.Transform(data);

            // We can also limit to 80% of explained variance:
            pca.ExplainedVariance = 0.8;

            // And then call transform again:
            double[][] output3 = pca.Transform(data);
            #endregion

            actual = pca.Transform(data);

            // transformedData shown in pg. 18
            expected = new double[,]
            {
                {  0.827970186 },
                { -1.77758033, },
                {  0.992197494 },
                {  0.274210416 },
                {  1.67580142, },
                {  0.912949103 },
                { -0.099109437 },
                { -1.14457216, },
                { -0.438046137 },
                { -1.22382056, },
            }.Multiply(-1);

            // Everything is correct (up to 8 decimal places)
            Assert.IsTrue(expected.IsEqual(actual, atol: 1e-8));


            // Create the analysis using the selected method
            pca = new KernelPrincipalComponentAnalysis()
            {
                Kernel = new Linear(),
                Method = method,
                NumberOfOutputs = 1
            };

            // Compute it
            pca.Learn(data);

            actual = pca.Transform(data);

            // transformedData shown in pg. 18
            expected = new double[,]
            {
                {  0.827970186 },
                { -1.77758033, },
                {  0.992197494 },
                {  0.274210416 },
                {  1.67580142, },
                {  0.912949103 },
                { -0.099109437 },
                { -1.14457216, },
                { -0.438046137 },
                { -1.22382056, },
            }.Multiply(-1);

            // Everything is correct (up to 8 decimal places)
            Assert.IsTrue(expected.IsEqual(actual, atol: 1e-8));

        }

        [Test]
        public void learn_kernel_matrix()
        {
            #region doc_learn_kernel_matrix
            // This example shows how to compute KPCA from an already existing kernel matrix. Note that, 
            // in most cases, you can just pass your data samples and a choice of kernel function to KPCA 
            // and it will compute the kernel matrix internally for you. However, if you have already computed 
            // the kernel matrix, you can use the method shown below to avoid computing it twice.

            // Let's say those were our original data points
            double[][] data =
            {
                new double[] { 2.5,  2.4 },
                new double[] { 0.5,  0.7 },
                new double[] { 2.2,  2.9 },
                new double[] { 1.9,  2.2 },
                new double[] { 3.1,  3.0 },
                new double[] { 2.3,  2.7 },
                new double[] { 2.0,  1.6 },
                new double[] { 1.0,  1.1 },
                new double[] { 1.5,  1.6 },
                new double[] { 1.1,  0.9 }
            };

            // Let's say we have already computed their kernel 
            // matrix as part of some previous computation:
            double[] mean = data.Mean(dimension: 0);
            double[][] x = data.Subtract(mean, dimension: 0);

            Linear kernel = new Linear();
            double[][] K = kernel.ToJagged(x);


            // Assuming that we already have the kernel matrix K at this point, we can create the KPCA as
            var pca = new KernelPrincipalComponentAnalysis(kernel, PrincipalComponentMethod.KernelMatrix);

            // Compute it
            pca.Learn(K); // note: we pass the kernel matrix instead of the data points

            // Those are the expected eigenvalues, in descending order:
            double[] eigenvalues = pca.Eigenvalues.Divide(data.GetLength(0) - 1); //  { 1.28402771, 0.0490833989 };

            // And this will be their proportion:
            double[] proportions = pca.ComponentProportions; // { 0.963181314348646, 0.03681868565135403 };

            // We can transform the inputs using
            double[][] actual = pca.Transform(K);

            // The output should be similar to
            double[,] expected = new double[,]
            {
                {  0.827970186, -0.175115307 },
                { -1.77758033,   0.142857227 },
                {  0.992197494,  0.384374989 },
                {  0.274210416,  0.130417207 },
                {  1.67580142,  -0.209498461 },
                {  0.912949103,  0.175282444 },
                { -0.099109437, -0.349824698 },
                { -1.14457216,   0.046417258 },
                { -0.438046137,  0.017764629 },
                { -1.22382056,  -0.162675287 },
            }.Multiply(-1);



            // Now we can transform new data using KPCA by 
            // again feeding a kernel matrix manually:
            double[][] newData =
            {
                new double[] { 2.2,  2.7 },
                new double[] { 1.2,  4.9 },
                new double[] { 1.8,  0.2 },
            };

            // Subtract the mean before computing a kernel matrix
            double[][] y = newData.Subtract(mean, dimension: 0);

            // Create the kernel matrix for new data
            double[][] newK = kernel.ToJagged2(y, x);

            // Transform using the new kernel matrix
            double[][] output = pca.Transform(newK);

            // Output will be similar to
            // output = 
            // {
            //     new double[] { -0.845161763306007, -0.24880030917481 },
            //     new double[] { -1.78468140697569,  -2.47530044148084 },
            //     new double[] {  1.26393423496622,   1.15181172492746 }
            // };

            // We can project to just its first components by 
            // setting NumberOfOutputs to the desired components:

            pca.NumberOfOutputs = 1;

            // And then calling transform again:
            double[][] output2 = pca.Transform(newK);

            // We can also limit to 80% of explained variance:
            pca.ExplainedVariance = 0.8;

            // And then call transform again:
            double[][] output3 = pca.Transform(newK);
            #endregion

            // Everything is correct (up to 8 decimal places)
            Assert.IsTrue(expected.IsEqual(actual, atol: 1e-8));

            double[] expectedEigenvalues = { 1.28402771, 0.0490833989 };
            double[] expectedProportions = eigenvalues.Divide(eigenvalues.Sum());

            Assert.IsTrue(proportions.IsEqual(expectedProportions, rtol: 1e-9));
            Assert.IsTrue(eigenvalues.IsEqual(expectedEigenvalues, rtol: 1e-5));

            var reference = new PrincipalComponentAnalysis(PrincipalComponentMethod.Center);

            reference.Learn(data);

            double[][] expected1 = reference.Transform(newData).Multiply(-1);

            reference.NumberOfOutputs = 1;
            double[][] expected2 = reference.Transform(newData).Multiply(-1);

            reference.ExplainedVariance = 0.8;
            double[][] expected3 = reference.Transform(newData).Multiply(-1);

            Assert.IsTrue(expected1.IsEqual(output, 1e-6));
            Assert.IsTrue(expected2.IsEqual(output2, 1e-6));
            Assert.IsTrue(expected3.IsEqual(output3, 1e-6));
        }

        [Test]
        public void learn_whiten_success()
        {
            double[,] data =
            {
                { 2.5,  2.4 },
                { 0.5,  0.7 },
                { 2.2,  2.9 },
                { 1.9,  2.2 },
                { 3.1,  3.0 },
                { 2.3,  2.7 },
                { 2.0,  1.6 },
                { 1.0,  1.1 },
                { 1.5,  1.6 },
                { 1.1,  0.9 }
            };

            var method = PrincipalComponentMethod.Center; // PrincipalComponentMethod.Standardize
            var pca = new KernelPrincipalComponentAnalysis(new Linear(), method, whiten: true);

            pca.Learn(data);

            double[] eigenvalues = { 1.28402771, 0.0490833989 };
            double[] proportion = eigenvalues.Divide(eigenvalues.Sum());
            double[,] eigenvectors =
            {
                { 0.19940687993951403, -1.1061252858739095 },
                { 0.21626410214440508,  1.0199057073792104 }
            };

            // Everything is alright (up to the 9 decimal places shown in the tutorial)
            // Assert.IsTrue(eigenvectors.IsEqual(pca.ComponentMatrix, rtol: 1e-9));
            Assert.IsTrue(proportion.IsEqual(pca.ComponentProportions, rtol: 1e-9));
            Assert.IsTrue(eigenvalues.IsEqual(pca.Eigenvalues.Divide(data.GetLength(0) - 1), rtol: 1e-5));

            double[,] actual = pca.Transform(data);

            double[][] expected = new double[][]
            {
                new double[] {  0.243560157209023,  -0.263472650637184  },
                new double[] { -0.522902576315494,   0.214938218565977  },
                new double[] {  0.291870144299372,   0.578317788814594  },
                new double[] {  0.0806632088164338,  0.19622137941132   },
                new double[] {  0.492962746459375,  -0.315204397734004  },
                new double[] {  0.268558011864442,   0.263724118751361  },
                new double[] { -0.0291545644762578, -0.526334573603598  },
                new double[] { -0.336693495487974,   0.0698378585807067 },
                new double[] { -0.128858004446015,   0.0267280693333571 },
                new double[] { -0.360005627922904,  -0.244755811482527  }
            }.Multiply(-1);

            // Everything is correct (up to 8 decimal places)
            Assert.IsTrue(expected.IsEqual(actual, atol: 1e-8));
        }

#if !NO_BINARY_SERIALIZATION
        [Test]
        [Category("Serialization")]
        public void SerializeTest()
        {
            double[][] actual, expected = new double[][]
            {
                new double[] { -0.57497881446526, 0.0385634996866008 },
                new double[] { 0.615576484818799, 0.463702189462175 },
                new double[] { -0.593829949403244, 0.225530142279202 },
                new double[] { -0.317225395167446, -0.470473936541537 },
                new double[] { -0.372910594880684, 0.490761790918429 },
                new double[] { -0.635435167456034, 0.154118097007375 },
                new double[] { 0.0591892009931548, -0.659772491267272 },
                new double[] { 0.739020019118434, 0.133927036952283 },
                new double[] { 0.344183901764118, -0.559832164574368 },
                new double[] { 0.736410314678163, 0.183475836077113 }
            };



            var target = new KernelPrincipalComponentAnalysis()
            {
                Kernel = new Gaussian(0.7)
            };

            target.Learn(data.ToJagged());

            actual = target.Transform(data.ToJagged());
            string str = actual.ToCSharp();
            Assert.IsTrue(Matrix.IsEqual(actual, expected, 1e-10));

            var copy = Serializer.DeepClone(target);

            actual = copy.Transform(data.ToJagged());
            Assert.IsTrue(Matrix.IsEqual(actual, expected, 1e-10));

            Assert.IsTrue(target.Kernel.Equals(copy.Kernel));
            Assert.IsTrue(target.ComponentProportions.IsEqual(copy.ComponentProportions));
            Assert.IsTrue(target.ComponentVectors.IsEqual(copy.ComponentVectors));
            Assert.IsTrue(target.CumulativeProportions.IsEqual(copy.CumulativeProportions));
            Assert.IsTrue(target.Eigenvalues.IsEqual(copy.Eigenvalues));
            Assert.IsTrue(target.MaximumNumberOfOutputs.IsEqual(copy.MaximumNumberOfOutputs));
            Assert.IsTrue(target.Method.Equals(copy.Method));
            Assert.IsTrue(target.NumberOfInputs.IsEqual(copy.NumberOfInputs));
            Assert.IsTrue(target.NumberOfOutputs.IsEqual(copy.NumberOfOutputs));
            Assert.IsTrue(target.Overwrite.Equals(copy.Overwrite));
            Assert.IsTrue(target.Whiten.Equals(copy.Whiten));
        }
#endif
    }
}
