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
namespace Accord.Tests.Audio
{
    using Accord.Audio;
    using Accord.Audio.Formats;
    using SharpDX.Multimedia;
    using NUnit.Framework;
    using Accord.Audio.Windows;
    using Accord.Math;
    using System.IO;
    using Accord.DataSets;
    using System.Linq;
    using System;
    using System.Collections.Generic;

    [TestFixture]
    public class MFCCTests
    {
        public MFCCTests()
        {
            // Make sure we have loaded Accord.Audio.DirectSound
            Type t = typeof(Accord.DirectSound.AudioCaptureDevice);
        }

        [Test]
        public void sample_test()
        {
            string basePath = NUnit.Framework.TestContext.CurrentContext.TestDirectory;
            string pathWhereTheDatasetShouldBeStored = Path.Combine(basePath, "mfcc");

            #region doc_example1
            // Let's say we would like to analyse an audio sample. To give an example that
            // could be reproduced by anyone without having to give a specific sound file
            // that would need to have been downloaded by every user trying to run this example,
            // we will use obtain an example from the Free Spoken Digits Dataset instead:
            var fsdd = new FreeSpokenDigitsDataset(path: pathWhereTheDatasetShouldBeStored);

            // Let's obtain one of the audio signals:
            Signal a = fsdd.GetSignal(0, "jackson", 10);

            // Note: if you would like to load a signal from the 
            // disk, you could use the following method directly:
            // Signal a = Signal.FromFile(fileName);

            // First we could extract some characteristics from the audio signal, just 
            // for informative purposes. We don't actually need to register them just 
            // to compute the MFCC, so please skip those checks if you would like!
            int numberOfChannels = a.NumberOfChannels;  // should be: 1
            int numberOfFrames = a.NumberOfFrames;      // should be: 5451
            int numberOfSamples = a.NumberOfSamples;    // should be: 5451
            SampleFormat format = a.SampleFormat;       // should be: Format32BitIeeeFloat
            int sampleRate = a.SampleRate;              // should be: 8000 (8khz)
            int samples = a.Samples;                    // should be: 5451
            int sampleSize = a.SampleSize;              // should be: 4
            int numberOfBytes = a.NumberOfBytes;        // should be: 21804

            // Now, let's say we would like to compute its MFCC:
            var extractor = new MelFrequencyCepstrumCoefficient(
                filterCount: 40,            // Note: all values are optional, you can 
                cepstrumCount: 13,          // specify only the ones you'd like and leave
                lowerFrequency: 133.3333,   // all others at their defaults
                upperFrequency: 6855.4976,
                alpha: 0.97,
                samplingRate: 16000,
                frameRate: 100,
                windowLength: 0.0256,
                numberOfBins: 512);

            // We can call the transform method of the MFCC extractor class:
            IEnumerable < MelFrequencyCepstrumCoefficientDescriptor> mfcc = extractor.Transform(a);

            // or we could also transform them to a matrix directly with:
            double[][] actual = mfcc.Select(x => x.Descriptor).ToArray();

            // This matrix would contain X different MFCC values (due the length of the signal)
            int numberOfMFCCs = actual.Length; // should be 35 (depends on the MFCC window)

            // Each of those MFCC values would have length 13;
            int descriptorLength = actual[0].Length; // 13 (depends on the MFCC Ceptrtum's count)

            // An example of an MFCC vector would have been:
            double[] row = actual[0]; // should have been: (see vector written below)

            double[] expected = new double[]
            {
                10.570020645259348d, 1.3484344242338475d, 0.4861056552885234d,
                -0.79287993818868352d, -0.64182784362935996d, -0.28079835895392041d,
                -0.46378109632237779d, 0.072039410871952647d, -0.43971730320461733d,
                0.48891921252102533d, -0.22502241185050212d, 0.12478713268421229d, -0.13373400147110801d
            };
            #endregion

            Assert.AreEqual(1, numberOfChannels);
            Assert.AreEqual(5451, numberOfFrames);
            Assert.AreEqual(5451, numberOfSamples);
            Assert.AreEqual(SampleFormat.Format32BitIeeeFloat, format);
            Assert.AreEqual(8000, sampleRate);
            Assert.AreEqual(5451, samples);
            Assert.AreEqual(4, sampleSize);
            Assert.AreEqual(21804, numberOfBytes);
            Assert.AreEqual(sampleSize * numberOfFrames * numberOfChannels, numberOfBytes);
            Assert.AreEqual(35, numberOfMFCCs);
            Assert.IsTrue(expected.IsEqual(row, 1e-8));

            Signal b = fsdd.GetSignal(0, "nicolas", 10);
            Assert.AreEqual(2, b.NumberOfChannels);
            Assert.AreEqual(3755, b.NumberOfFrames);
            Assert.AreEqual(7510, b.NumberOfSamples);
            Assert.AreEqual(SampleFormat.Format32BitIeeeFloat, b.SampleFormat);
            Assert.AreEqual(8000, b.SampleRate);
            Assert.AreEqual(7510, b.Samples);
            Assert.AreEqual(4, b.SampleSize);
            Assert.AreEqual(30040, b.NumberOfBytes);
            Assert.AreEqual(b.SampleSize * b.NumberOfFrames * b.NumberOfChannels, b.NumberOfBytes);
            MelFrequencyCepstrumCoefficientDescriptor[] rb = extractor.Transform(b).ToArray();
            Assert.AreEqual(24, rb.Length);
            Assert.IsTrue(new[] { 10.6434445230168, -0.222107787197107, 0.316067614396639, -0.212769536249701, -0.107755264262885, -0.292732772820073, -0.00445205345925395, 0.024397440969199, 0.0213769364471326, -0.0882765552657509, -0.177682484734242, -0.1013307739251, -0.099014915302743 }.IsEqual(rb[0].Descriptor, 1e-8));

            Signal c = fsdd.GetSignal(5, "theo", 23);
            Assert.AreEqual(1, c.NumberOfChannels);
            Assert.AreEqual(4277, c.NumberOfFrames);
            Assert.AreEqual(4277, c.NumberOfSamples);
            Assert.AreEqual(SampleFormat.Format32BitIeeeFloat, c.SampleFormat);
            Assert.AreEqual(8000, c.SampleRate);
            Assert.AreEqual(4277, c.Samples);
            Assert.AreEqual(4, c.SampleSize);
            Assert.AreEqual(17108, c.NumberOfBytes);
            Assert.AreEqual(b.SampleSize * c.NumberOfFrames * c.NumberOfChannels, c.NumberOfBytes);
            MelFrequencyCepstrumCoefficientDescriptor[] rc = extractor.Transform(c).ToArray();
            Assert.AreEqual(27, rc.Length);
            Assert.IsTrue(new[] { 7.24614406589037, -1.16796769512142, -0.134374026111248, -0.192703972718674, 0.112752647291759, -0.118712048308068, -0.0603752892245708, -0.0275002195634854, -0.0830858413953528, -0.0838965948140795, -0.15293502718595, 0.0107796827068413, -0.0491283773795346 }.IsEqual(rc[0].Descriptor, 1e-8));
        }

        [Test]
        public void sig2s2mfcTest()
        {
            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "sa1.wav");
            Signal w_sig = loadSignalFromWaveFile(fileName);

            // test for the default result
            MFCC w_mfcc = new MFCC();
            double[,] w_expectedMfccVal =
            {
                {3.91389903, -0.63428996, -0.73083372, -0.27443219, -0.54074218, -0.21305643, -0.33009162,  0.01223665, 0.45162122, 0.15050475, -0.09619379, -0.13371888, 0.18935507},
                {3.69534644, -0.68937544, -0.60949647, -0.09637816, -0.31463132, -0.32314598, -0.41129398, -0.21011665, 0.06462785, -0.07624873, -0.20837106, -0.14339344, 0.07252984},
                {3.25482565e+00, -7.15718205e-01, -5.04548111e-01, -1.97574580e-01, -3.15717510e-01, -1.74628550e-01, -3.20555008e-01, -3.06257583e-03, 1.83161036e-01, 8.65540658e-02, -9.35392446e-02, -1.52130545e-01, 4.03793471e-03},
                {3.07159893, -0.62133048, -0.37605836, -0.18068081, -0.21762302, -0.18910972, -0.32020514, -0.07012279,  0.12233751, 0.02942084, -0.04949337, -0.04563318, 0.11019492 },
                {2.74340933e+00, -7.08988732e-01, -2.99531037e-01, -1.98055215e-01, -3.02207415e-01, -3.13485271e-01, -2.80700575e-01, 1.67755943e-02, 1.61175304e-01, -2.20894251e-04, -4.25688705e-02, -9.82638399e-02, 3.37645901e-02}
            };
            double[][] w_actualMfccSig = w_mfcc.ProcessSignal(w_sig);

            Assert.AreEqual(w_actualMfccSig.Rows(), 283);
            Assert.AreEqual(w_actualMfccSig.Columns(), 13);

            for (int w_i = 0; w_i < 5; w_i++)
                for (int w_j = 0; w_j < 13; w_j++)
                    Assert.AreEqual(w_expectedMfccVal[w_i, w_j], w_actualMfccSig[w_i][w_j], 0.000001);

            //test for MFCC parameter nfilt = 25, ncep = 12, lowerf = 130.0, upperf = 6800.0, alpha = 0.87
            w_mfcc = new MFCC(25, 12, 130.0, 6800.0, 0.87);
            double[,] w_expectedMfccVal_1 =
            {
                {4.13175446, -0.45746458, -0.65432219, -0.2140113, -0.49878507, -0.26639248, -0.43689749, 0.00970135, 0.41347535, 0.09463917, -0.07951737, -0.06289812},
                {3.90927554, -0.50781141, -0.47409891, 0.01831778, -0.23448214, -0.29941623, -0.41492151, -0.17115635, 0.08229384, -0.09572194, -0.19925789, -0.11871483},
                {3.4429235, -0.52405457, -0.37301593, -0.11474238, -0.24959892, -0.16173184, -0.32337604, -0.00942962, 0.1586781, 0.05343699, -0.10842342, -0.12708318},
                {3.26886752, -0.44637767, -0.23872891, -0.09883555, -0.17149914, -0.23496224, -0.37151199, -0.0946323, 0.08348784, -0.02832338, -0.06774762, -0.03713541},
                {2.88170975, -0.54571717, -0.19885646, -0.10661848, -0.24702927, -0.273381, -0.27160955, 0.00899187, 0.16025249, 0.00751152, -0.03147983, -0.07606966}
            };

            double[][] w_actualMfccSig_1 = w_mfcc.ProcessSignal(w_sig);

            Assert.AreEqual((int)w_actualMfccSig_1.Columns(), (int)12);

            for (int w_i = 0; w_i < 5; w_i++)
                for (int w_j = 0; w_j < 12; w_j++)
                    Assert.AreEqual(w_expectedMfccVal_1[w_i, w_j], w_actualMfccSig_1[w_i][w_j], 0.000001);

            //test for MFCC parameter nfilt = 25, ncep = 12, lowerf = 130.0, upperf = 6800.0, alpha = 0.87
            w_mfcc = new MFCC(35, 12, 130.0, 6890.0, 0.97);
            double[,] w_expectedMfccVal_2 =
            {
                {3.95757826, -0.59464561, -0.69878593, -0.25837055, -0.55493371, -0.25020778, -0.37585643, -0.02871867, 0.42886297, 0.12878995, -0.11080583, -0.11860296},
                {3.76931287, -0.6459003, -0.56337162, -0.04560939, -0.29495178, -0.32606449, -0.43379464, -0.21751486, 0.06950738, -0.08252954, -0.21313119, -0.13387352},
                {3.31044931, -0.6881805, -0.48189091, -0.16050655, -0.29858016, -0.17695709, -0.33559796, -0.02962035, 0.16261671, 0.06071229, -0.13669495, -0.16597},
                {3.14036623, -0.58641167, -0.33283952, -0.15307373, -0.21683636, -0.22139794, -0.36191339, -0.11101222, 0.08726032, -0.00426344, -0.07244142, -0.03946722},
                {2.80633631e+00, -6.80484182e-01, -2.83551364e-01, -1.66453772e-01, -2.89528527e-01, -3.02580033e-01, -2.85276035e-01, -3.18950002e-04, 1.80481693e-01, 5.06706047e-03, -6.60448306e-02, -1.01470709e-01}
            };

            double[][] w_actualMfccSig_2 = w_mfcc.ProcessSignal(w_sig);

            Assert.AreEqual((int)w_actualMfccSig_2.Columns(), (int)12);

            for (int w_i = 0; w_i < 5; w_i++)
                for (int w_j = 0; w_j < 12; w_j++)
                    Assert.AreEqual(w_expectedMfccVal_2[w_i, w_j], w_actualMfccSig_2[w_i][w_j], 0.000001);
        }

        private static Signal loadSignalFromWaveFile(string p_filePath)
        {
            WaveDecoder sourceDecoder = new WaveDecoder(p_filePath);

            // Decode the file and store into a signal
            Signal sourceSignal = sourceDecoder.Decode();

            return sourceSignal;
        }

        [Test]
        public void logspec2s2mfcTest()
        {
            double[,] input =
            {
                { 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549 },
                { 0.996078431372549, 0.996078431372549, 0.862745098039216, 0, 0.662745098039216, 0.996078431372549, 0.996078431372549, 0.996078431372549 },
                { 0.996078431372549, 0.996078431372549, 0.529411764705882, 0.129411764705882, 0.262745098039216, 0.996078431372549, 0.996078431372549, 0.996078431372549 },
                { 0.996078431372549, 0.996078431372549, 0.0627450980392157, 0.662745098039216, 0.0627450980392157, 0.862745098039216, 0.996078431372549, 0.996078431372549 }
            };

            double[,] actual = MFCC.logspec2s2mfc(input, 5);

            double[,] expected =
            {
                {0.93382353, -0.06105869, -0.05751603, -0.05176306, -0.04402086},
                {0.75098039, -0.08648009,  0.10238907,  0.01060854, -0.14974026},
                {0.6754902,  -0.09671846,  0.14958308,  0.01470892, -0.14419432},
                {0.64215686, -0.10198394,  0.13978935,  0.0046477,  -0.06169853}
            };

            Assert.AreEqual(actual.GetLength(0), 4);
            Assert.AreEqual(actual.GetLength(1), 5);

            for (int w_i = 0; w_i < expected.GetLength(0); w_i++)
            {
                for (int w_j = 0; w_j < expected.GetLength(1); w_j++)
                    Assert.AreEqual(expected[w_i, w_j], actual[w_i, w_j], 0.000001);
            }
        }

        [Test]
        public void dctTest()
        {
            double[,] input =
            {
                { 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549 },
                { 0.996078431372549, 0.996078431372549, 0.862745098039216, 0, 0.662745098039216, 0.996078431372549, 0.996078431372549, 0.996078431372549 },
                { 0.996078431372549, 0.996078431372549, 0.529411764705882, 0.129411764705882, 0.262745098039216, 0.996078431372549, 0.996078431372549, 0.996078431372549 },
                { 0.996078431372549, 0.996078431372549, 0.0627450980392157, 0.662745098039216, 0.0627450980392157, 0.862745098039216, 0.996078431372549, 0.996078431372549 },
                { 0.996078431372549, 0.662745098039216, 0, 0, 0, 0.529411764705882, 0.996078431372549, 0.996078431372549 },
                { 0.996078431372549, 0.262745098039216, 0.529411764705882, 0.996078431372549, 0.729411764705882, 0.0627450980392157, 0.996078431372549, 0.996078431372549 },
                { 0.862745098039216, 0, 0.929411764705882, 0.996078431372549, 0.996078431372549, 0.129411764705882, 0.662745098039216, 0.996078431372549 },
                { 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549 }
            };

            double[,] actual = MFCC.dct(input);

            double[,] expected =
            {
                {2.81733525e+00, 1.66533454e-16, -1.66533454e-16, 3.33066907e-16, 1.11022302e-16, -2.22044605e-16, -2.16493490e-15, 3.35842465e-15, -2.89805495e-15, 1.27675648e-15, -1.38777878e-16, 1.11022302e-16, -5.55111512e-16},
                {2.30017676e+00,  -1.01685593e-01,  6.39620389e-01,  2.49486410e-01, -4.22877585e-01, -2.88532226e-01,  1.92779960e-01,  2.69574011e-01, -2.79684150e-15, -2.69574011e-01, -1.92779960e-01,  2.88532226e-01, 4.22877585e-01},
                {2.08665825e+00, -1.42639076e-01, 8.28396427e-01, 2.65887914e-01, -4.00693843e-01, -1.00952383e-01, 9.05748550e-02, -1.28623891e-01, -2.98582131e-15, 1.28623891e-01, -9.05748550e-02, 1.00952383e-01,  4.00693843e-01},
                {1.99237734e+00,  -1.63700997e-01,   7.89221535e-01,   2.25643042e-01,  -7.07106781e-02,   1.71404755e-01,  -2.50369577e-01,  -6.26823429e-01,  -3.06334101e-15,   6.26823429e-01,   2.50369577e-01,  -1.71404755e-01,   7.07106781e-02},
                {1.47799182e+00,  -2.85640977e-01,   1.13636006e+00,   2.92134687e-01,  -6.93241942e-02,   1.11822658e-01,  -1.40537494e-01,  -1.27499859e-01,  -2.50926526e-15,   1.27499859e-01,   1.40537494e-01,  -1.11822658e-01,   6.93241942e-02},
                {1.96880712e+00,  -1.49227094e-01,   2.50745082e-01,  -2.31392812e-01,   6.59966329e-01,   5.16004960e-01,  -2.56935387e-01,   2.66947291e-01,  -1.83232338e-15,  -2.66947291e-01,   2.56935387e-01,  -5.16004960e-01,  -6.59966329e-01},
                {1.97019360e+00,  -1.18683797e-01,  -1.37378296e-01,  -3.83097842e-01,   7.52860749e-01,   3.66003432e-01,   1.57452149e-01,   5.03682548e-01,  -1.59583599e-15,  -5.03682548e-01,  -1.57452149e-01,  -3.66003432e-01,  -7.52860749e-01},
                {2.81733525e+00,   1.66533454e-16,  -1.66533454e-16,   3.33066907e-16,   1.11022302e-16,  -2.22044605e-16,  -2.16493490e-15,  3.35842465e-15,  -2.89805495e-15,   1.27675648e-15,  -1.38777878e-16,   1.11022302e-16,  -5.55111512e-16}
            };

            Assert.AreEqual(actual.GetLength(0), 8);
            Assert.AreEqual(actual.GetLength(1), 13);

            for (int w_i = 0; w_i < expected.GetLength(0); w_i++)
            {
                for (int w_j = 0; w_j < expected.GetLength(1); w_j++)
                    Assert.AreEqual(expected[w_i, w_j], actual[w_i, w_j], 0.000001);
            }
        }

        [Test]
        public void dct2Test()
        {
            double[,] input =
            {
                { 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549 },
                { 0.996078431372549, 0.996078431372549, 0.862745098039216, 0, 0.662745098039216, 0.996078431372549, 0.996078431372549, 0.996078431372549 },
                { 0.996078431372549, 0.996078431372549, 0.529411764705882, 0.129411764705882, 0.262745098039216, 0.996078431372549, 0.996078431372549, 0.996078431372549 },
                { 0.996078431372549, 0.996078431372549, 0.0627450980392157, 0.662745098039216, 0.0627450980392157, 0.862745098039216, 0.996078431372549, 0.996078431372549 },
                { 0.996078431372549, 0.662745098039216, 0, 0, 0, 0.529411764705882, 0.996078431372549, 0.996078431372549 },
                { 0.996078431372549, 0.262745098039216, 0.529411764705882, 0.996078431372549, 0.729411764705882, 0.0627450980392157, 0.996078431372549, 0.996078431372549 },
                { 0.862745098039216, 0, 0.929411764705882, 0.996078431372549, 0.996078431372549, 0.129411764705882, 0.662745098039216, 0.996078431372549 },
                { 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549, 0.996078431372549 }
            };

            double[,] actual = MFCC.dct2(input);

            double[,] expected =
            {
                {1.99215686e+00,   8.32667268e-17,  -8.32667268e-17,   1.66533454e-16,   5.55111512e-17,  -1.11022302e-16,  -1.08246745e-15,   1.67921232e-15,  -1.44902748e-15,   6.38378239e-16,  -6.93889390e-17,   5.55111512e-17,  -2.77555756e-16},
                {1.62647059e+00,  -5.08427964e-02,   3.19810194e-01,   1.24743205e-01,  -2.11438792e-01,  -1.44266113e-01,   9.63899799e-02,   1.34787005e-01,  -1.39842075e-15,  -1.34787005e-01,  -9.63899799e-02,   1.44266113e-01,   2.11438792e-01},
                {1.47549020e+00,  -7.13195379e-02,   4.14198213e-01,   1.32943957e-01,  -2.00346921e-01,  -5.04761913e-02,   4.52874275e-02,  -6.43119454e-02,  -1.49291065e-15,   6.43119454e-02,  -4.52874275e-02,   5.04761913e-02,   2.00346921e-01},
                {1.40882353e+00,  -8.18504983e-02,   3.94610767e-01,   1.12821521e-01,  -3.53553391e-02,   8.57023774e-02,  -1.25184788e-01,  -3.13411715e-01,  -1.53167050e-15,   3.13411715e-01,   1.25184788e-01,  -8.57023774e-02,   3.53553391e-02},
                {1.04509804e+00,  -1.42820489e-01,   5.68180030e-01,   1.46067343e-01,  -3.46620971e-02,   5.59113288e-02,  -7.02687468e-02,  -6.37499293e-02,  -1.25463263e-15,   6.37499293e-02,   7.02687468e-02,  -5.59113288e-02,   3.46620971e-02},
                {1.39215686e+00,  -7.46135469e-02,   1.25372541e-01,  -1.15696406e-01,   3.29983165e-01,   2.58002480e-01,  -1.28467693e-01,   1.33473645e-01,  -9.16161690e-16,  -1.33473645e-01,   1.28467693e-01,  -2.58002480e-01,  -3.29983165e-01},
                {1.39313725e+00,  -5.93418985e-02,  -6.86891478e-02,  -1.91548921e-01,   3.76430375e-01,   1.83001716e-01,   7.87260746e-02,   2.51841274e-01,  -7.97917996e-16,  -2.51841274e-01,  -7.87260746e-02,  -1.83001716e-01,  -3.76430375e-01},
                {1.99215686e+00,   8.32667268e-17,  -8.32667268e-17,   1.66533454e-16,   5.55111512e-17,  -1.11022302e-16,  -1.08246745e-15,   1.67921232e-15,  -1.44902748e-15,   6.38378239e-16,  -6.93889390e-17,   5.55111512e-17,  -2.77555756e-16}
            };

            Assert.AreEqual(actual.GetLength(0), 8);
            Assert.AreEqual(actual.GetLength(1), 13);

            for (int w_i = 0; w_i < expected.GetLength(0); w_i++)
            {
                for (int w_j = 0; w_j < expected.GetLength(1); w_j++)
                    Assert.AreEqual(expected[w_i, w_j], actual[w_i, w_j], 0.000001);
            }
        }

        [Test]
        public void idctTest()
        {
            double[,] input =
            {
                {  6.1917, -0.3411,  1.2418,  0.1492},
                {  0.2205,  0.0214,  0.4503,  0.3947},
                {  1.0423,  0.2214, -1.0017, -0.2720},
                { -0.2340, -0.0392, -0.2617, -0.2866}
            };

            double[,] actual = MFCC.idct(input, 5);

            double[,] expected =
            {
                {3.25469367,  2.30976844,  1.98362914,  2.74286284,  3.55410801},
                {0.48861544, -0.21885283, -0.18618413,  0.24006121,  0.1694133},
                {-0.01434953,  0.90781576,  1.09966144,  0.41598921, -0.07846322},
                {-0.36867313,  0.10431619,  0.06086563, -0.21131911, -0.10842948}
            };

            Assert.AreEqual(actual.GetLength(0), 4);
            Assert.AreEqual(actual.GetLength(1), 5);

            for (int w_i = 0; w_i < expected.GetLength(0); w_i++)
            {
                for (int w_j = 0; w_j < expected.GetLength(1); w_j++)
                    Assert.AreEqual(expected[w_i, w_j], actual[w_i, w_j], 0.000001);
            }
        }

        [Test]
        public void dct3Test()
        {
            double[,] input =
            {
                {  6.1917, -0.3411,  1.2418,  0.1492},
                {  0.2205,  0.0214,  0.4503,  0.3947},
                {  1.0423,  0.2214, -1.0017, -0.2720},
                { -0.2340, -0.0392, -0.2617, -0.2866}
            };

            double[,] actual = MFCC.dct3(input, 5);

            double[,] expected =
            {
                {3.86377949,  2.36972151,  1.85405,     3.05450388,  4.33719512},
                {0.7269018,  -0.39170376, -0.34005,     0.33390305,  0.2221989},
                {-0.238556,    1.21951535,  1.52285,     0.4418693,  -0.33992865},
                {-0.53446042,  0.21340136,  0.1447,     -0.28566187, -0.12297908}
            };

            Assert.AreEqual(actual.GetLength(0), 4);
            Assert.AreEqual(actual.GetLength(1), 5);

            for (int w_i = 0; w_i < expected.GetLength(0); w_i++)
            {
                for (int w_j = 0; w_j < expected.GetLength(1); w_j++)
                    Assert.AreEqual(expected[w_i, w_j], actual[w_i, w_j], 0.000001);
            }
        }
    }
}