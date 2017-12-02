using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Kernels;
using Accord.Statistics;
using System;
using System.Diagnostics;
using System.Linq;
using Accord.Math.Optimization.Losses;
using Accord.MachineLearning;
using Accord.Statistics.Analysis;
using System.IO;
using Accord.IO;
using Accord.DataSets;
using Accord.Imaging.Converters;
using System.Drawing;
using Accord.Statistics.Distributions.DensityKernels;
using Accord.Vision.Detection;
using Accord.Video.FFMPEG;
using System.Drawing.Imaging;
using Accord.Imaging.Filters;
using Accord.Imaging;
using Accord.Audio.Generators;
using Accord.Audio;
using Accord.Video.DirectShow;
using System.Threading;
using System.Runtime.InteropServices;

namespace Accord.Perf.MachineLearning
{
    class Program
    {

        static void Main(string[] args)
        {
#if !NETSTANDARD2_0
            Trace.Listeners.Add(new ConsoleTraceListener());
#endif

            Trace.WriteLine("Running in " + (Environment.Is64BitProcess ? "x64" : "x86"));

            //TestSparseKernelSVM();
            //TestPredictSparseSVM();
            //TestSparseSVMComplete();
            //TestPredictSparseMulticlassSVM();
            //TestLinearASGD();
            //TestSMO();
            //TestMeanShift();
            //TestEmpty();
            //TestHaar();
            //TestFFMPEG();
            //TestFFMPEG2();
            TestCameraStartStop();
        }

        private static void TestSparseKernelSVM()
        {
            Console.WriteLine("Downloading dataset");
            var news20 = new Accord.DataSets.News20(@"C:\Temp\");
            Sparse<double>[] inputs = news20.Training.Item1.Get(0, 2000);
            int[] outputs = news20.Training.Item2.ToMulticlass().Get(0, 2000);

            var learn = new MultilabelSupportVectorLearning<Linear, Sparse<double>>()
            {
                // using LIBLINEAR's L2-loss SVC dual for each SVM
                Learner = (p) => new LinearDualCoordinateDescent<Linear, Sparse<double>>()
                {
                    Loss = Loss.L2,
                    Complexity = 1.0,
                    Tolerance = 1e-4
                }
            };

            Console.WriteLine("Learning");
            Stopwatch sw = Stopwatch.StartNew();
            var svm = learn.Learn(inputs, outputs);
            Console.WriteLine(sw.Elapsed);

            Console.WriteLine("Predicting");
            sw = Stopwatch.StartNew();
            int[] predicted = svm.ToMulticlass().Decide(inputs);
            Console.WriteLine(sw.Elapsed);
        }

        private static void TestPredictSparseSVM()
        {
            Console.WriteLine("Downloading dataset");
            var news20 = new Accord.DataSets.News20(@"C:\Temp\");
            Sparse<double>[] inputs = news20.Training.Item1;
            int[] outputs = news20.Training.Item2.ToMulticlass();

            var learn = new MultilabelSupportVectorLearning<Linear, Sparse<double>>()
            {
                // using LIBLINEAR's L2-loss SVC dual for each SVM
                Learner = (p) => new LinearDualCoordinateDescent<Linear, Sparse<double>>()
                {
                    Loss = Loss.L2,
                    Complexity = 1.0,
                    Tolerance = 1e-4
                }
            };

            Console.WriteLine("Learning");
            Stopwatch sw = Stopwatch.StartNew();
            var svm = learn.Learn(inputs.Get(0, 100), outputs.Get(0, 100));
            Console.WriteLine(sw.Elapsed);

            Console.WriteLine("Predicting");
            sw = Stopwatch.StartNew();
            int[] predicted = svm.ToMulticlass().Decide(inputs);
            Console.WriteLine(sw.Elapsed);
        }

        private static void TestPredictSparseMulticlassSVM()
        {
            Console.WriteLine("Downloading dataset");
            var news20 = new Accord.DataSets.News20(@"C:\Temp\");
            Sparse<double>[] inputs = news20.Training.Item1;
            int[] outputs = news20.Training.Item2.ToMulticlass();

            var learn = new MulticlassSupportVectorLearning<Linear, Sparse<double>>()
            {
                // using LIBLINEAR's L2-loss SVC dual for each SVM
                Learner = (p) => new LinearDualCoordinateDescent<Linear, Sparse<double>>()
                {
                    Loss = Loss.L2,
                    Complexity = 1.0,
                    Tolerance = 1e-4
                }
            };

            Console.WriteLine("Learning");
            Stopwatch sw = Stopwatch.StartNew();
            var svm = learn.Learn(inputs.Get(0, 1000), outputs.Get(0, 1000));
            Console.WriteLine(sw.Elapsed);

            Console.WriteLine("Predicting");
            sw = Stopwatch.StartNew();
            int[] predicted = svm.Decide(inputs);
            Console.WriteLine(sw.Elapsed);
        }

        private static void TestSparseSVMComplete()
        {
            #region doc_learn_news20
            Console.WriteLine("Downloading dataset:");
            var news20 = new Accord.DataSets.News20(@"C:\Temp\");
            var trainInputs = news20.Training.Item1;
            var trainOutputs = news20.Training.Item2.ToMulticlass();
            var testInputs = news20.Testing.Item1;
            var testOutputs = news20.Testing.Item2.ToMulticlass();

            Console.WriteLine(" - Training samples: {0}", trainInputs.Rows());
            Console.WriteLine(" - Testing samples: {0}", testInputs.Rows());
            Console.WriteLine(" - Dimensions: {0}", trainInputs.Columns());
            Console.WriteLine(" - Classes: {0}", trainOutputs.DistinctCount());
            Console.WriteLine();


            // Create and use the learning algorithm to train a sparse linear SVM
            var learn = new MultilabelSupportVectorLearning<Linear, Sparse<double>>()
            {
                // using LIBLINEAR's L2-loss SVC dual for each SVM
                Learner = (p) => new LinearDualCoordinateDescent<Linear, Sparse<double>>()
                {
                    Loss = Loss.L2,
                    Tolerance = 1e-4
                },
            };

            // Display progress in the console
            learn.SubproblemFinished += (sender, e) =>
            {
                Console.WriteLine(" - {0} / {1} ({2:00.0%})", e.Progress, e.Maximum, e.Progress / (double)e.Maximum);
            };

            // Start the learning algorithm
            Console.WriteLine("Learning");
            Stopwatch sw = Stopwatch.StartNew();
            var svm = learn.Learn(trainInputs, trainOutputs);
            Console.WriteLine("Done in {0}", sw.Elapsed);
            Console.WriteLine();


            // Compute accuracy in the training set
            Console.WriteLine("Predicting training set");
            sw = Stopwatch.StartNew();
            int[] trainPredicted = svm.ToMulticlass().Decide(trainInputs);
            Console.WriteLine("Done in {0}", sw.Elapsed);

            double trainError = new ZeroOneLoss(trainOutputs).Loss(trainPredicted);
            Console.WriteLine("Training error: {0}", trainError);
            Console.WriteLine();


            // Compute accuracy in the testing set
            Console.WriteLine("Predicting testing set");
            sw = Stopwatch.StartNew();
            int[] testPredicted = svm.ToMulticlass().Decide(testInputs);
            Console.WriteLine("Done in {0}", sw.Elapsed);

            double testError = new ZeroOneLoss(testOutputs).Loss(testPredicted);
            Console.WriteLine("Testing error: {0}", testError);
            #endregion
        }

        private static void TestLinearASGD()
        {
            // http://leon.bottou.org/projects/sgd

            string codebookPath = "codebook.bin";
            string x_train_fn = "x_train.txt.gz";
            string x_test_fn = "x_test.txt.gz";

            Sparse<double>[] xTrain = null, xTest = null;
            bool[] yTrain = null, yTest = null;

            // Check if we have the precomputed dataset on disk
            if (!File.Exists(x_train_fn) || !File.Exists(x_train_fn))
            {
                Console.WriteLine("Downloading dataset");
                RCV1v2 rcv1v2 = new RCV1v2(@"C:\Temp\");

                // Note: Leon Bottou's SGD inverts training and 
                // testing when benchmarking in this dataset
                var trainWords = rcv1v2.Testing.Item1;
                var testWords = rcv1v2.Training.Item1;

                string positiveClass = "CCAT";
                yTrain = rcv1v2.Testing.Item2.Apply(x => x.Contains(positiveClass));
                yTest = rcv1v2.Training.Item2.Apply(x => x.Contains(positiveClass));

                TFIDF tfidf;
                if (!File.Exists(codebookPath))
                {
                    Console.WriteLine("Learning TD-IDF");
                    // Create a TF-IDF considering only words that
                    // exist in both the training and testing sets
                    tfidf = new TFIDF(testWords)
                    {
                        Tf = TermFrequency.Log,
                        Idf = InverseDocumentFrequency.Default,
                    };

                    // Learn the training set
                    tfidf.Learn(trainWords);

                    Console.WriteLine("Saving codebook");
                    tfidf.Save(codebookPath);
                }
                else
                {
                    Console.WriteLine("Loading codebook");
                    Serializer.Load(codebookPath, out tfidf);
                }

                if (!File.Exists(x_train_fn))
                {
                    // Transform and normalize training set
                    Console.WriteLine("Pre-processing training set");
                    xTrain = tfidf.Transform(trainWords, out xTrain);

                    Console.WriteLine("Post-processing training set");
                    xTrain = xTrain.Divide(Norm.Euclidean(xTrain, dimension: 1), result: xTrain);

                    Console.WriteLine("Saving training set to disk");
                    SparseFormat.Save(xTrain, yTrain, x_train_fn, compression: SerializerCompression.GZip);
                }

                if (!File.Exists(x_test_fn))
                {
                    // Transform and normalize testing set
                    Console.WriteLine("Pre-processing testing set");
                    xTest = tfidf.Transform(testWords, out xTest);

                    Console.WriteLine("Post-processing testing set");
                    xTest = xTest.Divide(Norm.Euclidean(xTest, dimension: 1), result: xTest);

                    Console.WriteLine("Saving testing set to disk");
                    SparseFormat.Save(xTest, yTest, x_test_fn, compression: SerializerCompression.GZip);
                }
            }
            else
            {
                Console.WriteLine("Loading dataset from disk");
                if (xTrain == null || yTrain == null)
                    SparseFormat.Load(x_train_fn, out xTrain, out yTrain, compression: SerializerCompression.GZip);
                if (xTest == null || yTest == null)
                    SparseFormat.Load(x_test_fn, out xTest, out yTest, compression: SerializerCompression.GZip);
            }

            int positiveTrain = yTrain.Count(x => x);
            int positiveTest = yTest.Count(x => x);
            int negativeTrain = yTrain.Length - positiveTrain;
            int negativeTest = yTest.Length - positiveTest;

            Console.WriteLine("Training samples: {0} [{1}+, {2}-]", positiveTrain + negativeTrain, positiveTrain, negativeTrain);
            Console.WriteLine("Negative samples: {0} [{1}+, {2}-]", positiveTest + negativeTest, positiveTest, negativeTest);

            // Create and learn a linear sparse binary support vector machine
            var learn = new AveragedStochasticGradientDescent<Linear, Sparse<double>>()
            {
                MaxIterations = 5,
                Tolerance = 0,
            };

            Console.WriteLine("Learning training set");
            Stopwatch sw = Stopwatch.StartNew();
            var svm = learn.Learn(xTrain, yTrain);
            Console.WriteLine(sw.Elapsed);


            Console.WriteLine("Predicting training set");
            sw = Stopwatch.StartNew();
            bool[] trainPred = svm.Decide(xTrain);
            Console.WriteLine(sw.Elapsed);

            var train = new ConfusionMatrix(trainPred, yTrain);
            Console.WriteLine("Train acc: " + train.Accuracy);


            Console.WriteLine("Predicting testing set");
            sw = Stopwatch.StartNew();
            bool[] testPred = svm.Decide(xTest);
            Console.WriteLine(sw.Elapsed);

            var test = new ConfusionMatrix(testPred, yTest);
            Console.WriteLine("Test acc: " + test.Accuracy);
        }

        private static void TestSMO()
        {
            Console.WriteLine("Downloading dataset");
            var news20 = new Accord.DataSets.News20(@"C:\Temp\");
            Sparse<double>[] inputs = news20.Training.Item1.Get(0, 2000);
            int[] outputs = news20.Training.Item2.ToMulticlass().Get(0, 2000);

            var learn = new MultilabelSupportVectorLearning<Linear, Sparse<double>>()
            {
                // using LIBLINEAR's SVC dual for each SVM
                Learner = (p) => new SequentialMinimalOptimization<Linear, Sparse<double>>()
                {
                    Strategy = SelectionStrategy.SecondOrder,
                    Complexity = 1.0,
                    Tolerance = 1e-4,
                    CacheSize = 1000
                },
            };

            Console.WriteLine("Learning");
            Stopwatch sw = Stopwatch.StartNew();
            var svm = learn.Learn(inputs, outputs);
            Console.WriteLine(sw.Elapsed);

            Console.WriteLine("Predicting");
            sw = Stopwatch.StartNew();
            int[] predicted = svm.ToMulticlass().Decide(inputs);
            Console.WriteLine(sw.Elapsed);

            var test = new ConfusionMatrix(predicted, outputs);
            Console.WriteLine("Test acc: " + test.Accuracy);
        }

        static void TestMeanShift()
        {
            Bitmap image = Accord.Imaging.Image.FromUrl("https://c1.staticflickr.com/4/3209/2527630511_fae07530c2_b.jpg");

            //ImageBox.Show("Original", image).Hold();

            // Create converters to convert between Bitmap images and double[] arrays
            var imageToArray = new ImageToArray(min: -1, max: +1);
            var arrayToImage = new ArrayToImage(image.Width, image.Height, min: -1, max: +1);

            // Transform the image into an array of pixel values
            double[][] pixels; imageToArray.Convert(image, out pixels);


            // Create a MeanShift algorithm using given bandwidth
            //   and a Gaussian density kernel as kernel function.
            MeanShift meanShift = new MeanShift()
            {
                Kernel = new EpanechnikovKernel(),
                Bandwidth = 0.1,

                // We will compute the mean-shift algorithm until the means
                // change less than 0.05 between two iterations of the algorithm
                Tolerance = 0.05,
                MaxIterations = 10
            };

            // Learn the clusters from the data
            var clusters = meanShift.Learn(pixels);

            // Use clusters to decide class labels
            int[] labels = clusters.Decide(pixels);

            // Replace every pixel with its corresponding centroid
            double[][] replaced = pixels.Apply((x, i) => clusters.Modes[labels[i]]);

            // Retrieve the resulting image (shown in a picture box)
            Bitmap result; arrayToImage.Convert(replaced, out result);

            //ImageBox.Show("Mean-Shift clustering", result).Hold();
        }

        static void TestEmpty()
        {

        }

        static void TestHaar()
        {
            if (Environment.Is64BitProcess)
                throw new Exception("Run in 32-bits");

            // Let's test the detector using a sample video from 
            // the collection of test videos in the framework:
            TestVideos ds = new TestVideos();
            string fileName = ds["crowd.mp4"];

            // In this example, we will be creating a cascade for a Face detector:
            var cascade = new Accord.Vision.Detection.Cascades.FaceHaarCascade();

            // Now, create a new Haar object detector with the cascade:
            var detector = new HaarObjectDetector(cascade, minSize: 25,
                searchMode: ObjectDetectorSearchMode.Average,
                scalingMode: ObjectDetectorScalingMode.SmallerToGreater,
                scaleFactor: 1.1f)
            {
                Suppression = 5 // This should make sure we only report regions as faces if 
                // they have been detected at least 5 times within different cascade scales.
            };

            // Now, let's open the video using FFMPEG:
            var video = new VideoFileReader();
            video.Open(fileName);

            Stopwatch sw = Stopwatch.StartNew();

            // Now, for each frame of the video
            for (int frameIndex = 0; frameIndex < video.FrameCount; frameIndex++)
            {
                // Read the current frame into the bitmap data
                Bitmap bmp = video.ReadVideoFrame(frameIndex);

                // Feed the frame to the tracker
                Rectangle[] faces = detector.ProcessFrame(bmp);

                Console.WriteLine(faces.Length);
                Console.WriteLine(bmp.Flags);
            }

            sw.Stop();

            Console.WriteLine(sw.Elapsed);

            video.Close();
        }

        static void TestFFMPEG()
        {
            var videoWriter = new VideoFileWriter();

            int width = 800;
            int height = 600;
            int framerate = 24;
            string path = Path.GetFullPath("output.webm");
            int videoBitRate = 1200 * 1000;

            int audioFrameSize = 44100;
            int audioBitRate = 128000;
            int audioSampleRate = 44100;
            AudioLayout audioChannels = AudioLayout.Mono;


            videoWriter.Width = width;
            videoWriter.Height = height;
            videoWriter.FrameRate = framerate;
            videoWriter.VideoCodec = VideoCodec.Vp8;
            videoWriter.BitRate = videoBitRate;
            videoWriter.PixelFormat = AVPixelFormat.FormatYuv420P;
            videoWriter.Open(path);

            //, audioFrameSize, audioChannels, audioSampleRate, AudioCodec.Vorbis, audioBitRate);

            var a = new Accord.DirectSound.AudioDeviceCollection(DirectSound.AudioDeviceCategory.Capture);

            // Generate 1 second of audio
            SineGenerator gen = new SineGenerator()
            {
                SamplingRate = audioSampleRate,
                Channels = 1,
                Format = SampleFormat.Format16Bit,
                Frequency = 10,
                Amplitude = 1000.9f,
            };

            Signal s = gen.Generate(TimeSpan.FromSeconds(255));
            //s.Save("test.wav");

            var m2i = new MatrixToImage();
            Bitmap frame;

            for (byte i = 0; i < 255; i++)
            {
                byte[,] matrix = Matrix.Create(height, width, i);
                m2i.Convert(matrix, out frame);
                videoWriter.WriteVideoFrame(frame, TimeSpan.FromSeconds(1));

                //// Generate 1 second of audio
                //s = gen.Generate(TimeSpan.FromSeconds(1));
                //videoWriter.WriteAudioFrame(s);
            }

            videoWriter.Close();
        }

        static void TestFFMPEG2()
        {
            string outputPath = Path.GetFullPath("output.avi");

            // First, we create a new VideoFileWriter:
            var videoWriter = new VideoFileWriter()
            {
                // Our video will have the following characteristics:
                Width = 800,
                Height = 600,
                FrameRate = 24,
                BitRate = 1200 * 1000,
                VideoCodec = VideoCodec.Mpeg4,
                //PixelFormat = Accord.Video.FFMPEG.PixelFormat.FormatYUV420P
            };

            // We can open for it writing:
            videoWriter.Open(outputPath);

            // At this point, we can check the console of our application for useful 
            // information regarding our media streams created by FFMPEG. We can also
            // check those properties using the class itself, specially for properties
            // that we didn't set beforehand but that have been filled by FFMPEG:

            int width = videoWriter.Width;
            int height = videoWriter.Height;
            int frameRate = videoWriter.FrameRate.Numerator;
            int bitRate = videoWriter.BitRate;
            VideoCodec videoCodec = videoWriter.VideoCodec;

            // We haven't set those properties, but FFMPEG has filled them for us:
            AudioCodec audioCodec = videoWriter.AudioCodec;
            int audioSampleRate = videoWriter.SampleRate;
            AudioLayout audioChannels = videoWriter.AudioLayout;
            int numberOfChannels = videoWriter.NumberOfChannels;

            // Now, let's say we would like to save dummy images of changing color
            var m2i = new MatrixToImage();
            Bitmap frame;

            for (byte i = 0; i < 255; i++)
            {
                // Create bitmap matrix from a matrix of RGB values:
                byte[,] matrix = Matrix.Create(height, width, i);
                m2i.Convert(matrix, out frame);

                // Write the frame to the stream. We can optionally specify
                // the duration that this frame should remain in the stream:
                videoWriter.WriteVideoFrame(frame, TimeSpan.FromSeconds(i));
            }
        }

        static void TestCameraStartStop()
        {
            int i = 0;
            var collections = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            var _capture = new VideoCaptureDevice(collections[i].MonikerString);
            _capture.VideoResolution = _capture.VideoCapabilities.FirstOrDefault(item => System.Math.Abs(item.FrameSize.Width - 320) < 0.1 &&
                                                                                         System.Math.Abs(item.FrameSize.Height - 240) < 0.1);
            _capture.NewFrame += _captures_NewFrame;

            int steps = 300 * 30 * 3;
            do
            {
                _capture.Start();
                Thread.Sleep(100);
                _capture.SignalToStop();
                _capture.WaitForStop();
                i++;
                Marshal.CleanupUnusedObjectsInCurrentContext();
                GC.WaitForFullGCComplete();
                GC.GetTotalMemory(true);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                string str = string.Format("Test nr {0}/{1}, mem = {2} Mbytes", i, steps,
                    Process.GetCurrentProcess().PrivateMemorySize64 / 1024.0 / 1024.0);
                Console.WriteLine(str);

            } while (i < steps);
            Console.ReadKey();
        }

        private static void _captures_NewFrame(object sender, Accord.Video.NewFrameEventArgs eventargs)
        {
        }
    }
}
