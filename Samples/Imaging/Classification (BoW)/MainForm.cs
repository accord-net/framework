// Accord.NET Sample Applications
// http://accord-framework.net
//
// Copyright © 2009-2014, César Souza
// All rights reserved. 3-BSD License:
//
//   Redistribution and use in source and binary forms, with or without
//   modification, are permitted provided that the following conditions are met:
//
//      * Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer.
//
//      * Redistributions in binary form must reproduce the above copyright
//        notice, this list of conditions and the following disclaimer in the
//        documentation and/or other materials provided with the distribution.
//
//      * Neither the name of the Accord.NET Framework authors nor the
//        names of its contributors may be used to endorse or promote products
//        derived from this software without specific prior written permission.
// 
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Accord.Controls;
using Accord.Imaging;
using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Kernels;
using AForge;

namespace Classification.BoW
{
    /// <summary>
    ///   Image classification sample application.
    /// </summary>
    /// 
    /// <remarks>
    ///   This application uses the <see cref="BagOfVisualWords"/>,
    ///   <see cref="KernelSupportVectorMachines"/>, the <see cref="BinarySplit"/>
    ///   clustering algorithm and <see cref="SpeededUpRobustFeatures">SURF</see>
    ///   to perform image classification.
    /// </remarks>
    /// 
    public partial class MainForm : Form
    {

        Dictionary<string, Bitmap> originalTrainImages;
        Dictionary<string, Bitmap> originalTestImages;

        Dictionary<string, Bitmap> originalImages;
        Dictionary<string, Bitmap> displayImages;

        MulticlassSupportVectorMachine ksvm;


        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///   This method just loads the image datasets into memory.
        /// </summary>
        /// 
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Seed the number generator with a fixed number so results
            // are replicable across application runs. Comment the 
            // following line to generate truly random data splits.
            //
            Accord.Math.Tools.SetupGenerator(0);

            cbStrategy.DataSource = Enum.GetValues(typeof(SelectionStrategy));


            // Open Resources folder
            var path = new DirectoryInfo(Path.Combine(Application.StartupPath, "Resources"));

            // Create image list to load images into
            originalImages = new Dictionary<string, Bitmap>();
            displayImages = new Dictionary<string, Bitmap>();

            originalTestImages = new Dictionary<string, Bitmap>();
            originalTrainImages = new Dictionary<string, Bitmap>();

            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(64, 64);
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            listView1.LargeImageList = imageList;

            int currentClassLabel = 0;

            // For every class folder
            foreach (var classFolder in path.EnumerateDirectories())
            {
                string name = classFolder.Name;

                // Create two list view groups for each class.  Use 80%
                // of training instances and the remaining 20% as testing.
                ListViewGroup trainingGroup = listView1.Groups.Add(name + ".train", name + ".train");
                ListViewGroup testingGroup = listView1.Groups.Add(name + ".test", name + ".test");


                // For each file in the class folder
                FileInfo[] files = GetFilesByExtensions(classFolder, ".jpg", ".png").ToArray();

                // Shuffle the samples
                Accord.Statistics.Tools.Shuffle(files);

                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo file = files[i];

                    Bitmap image = (Bitmap)Bitmap.FromFile(file.FullName);

                    string shortName = file.Name;
                    string imageKey = file.FullName;

                    imageList.Images.Add(imageKey, image);
                    originalImages.Add(imageKey, image);
                    displayImages.Add(imageKey, image);

                    ListViewItem item;
                    if ((i / (double)files.Length) < 0.7)
                    {
                        item = new ListViewItem(trainingGroup);
                        originalTrainImages.Add(imageKey, image);
                    }
                    else
                    {
                        item = new ListViewItem(testingGroup);
                        originalTestImages.Add(imageKey, image);
                    }

                    item.ImageKey = imageKey;
                    item.Name = shortName;
                    item.Text = shortName;
                    item.Tag = new Tuple<double[], int>(null, currentClassLabel);

                    listView1.Items.Add(item);
                }

                currentClassLabel++;
            }
        }

        /// <summary>
        ///   This methods computes the Bag-of-Visual-Words with the training images.
        /// </summary>
        /// 
        private void btnBagOfWords_Click(object sender, EventArgs e)
        {
            int numberOfWords = (int)numWords.Value;

            // Create a Binary-Split clustering algorithm
            BinarySplit binarySplit = new BinarySplit(numberOfWords);

            // Create bag-of-words (BoW) with the given algorithm
            BagOfVisualWords bow = new BagOfVisualWords(binarySplit);

            if (cbExtended.Checked)
                bow.Detector.ComputeDescriptors = SpeededUpRobustFeatureDescriptorType.Extended;

            Stopwatch sw1 = Stopwatch.StartNew();

            // Compute the BoW codebook using training images only
            var points = bow.Compute(originalTrainImages.Values.ToArray());

            sw1.Stop();


            Stopwatch sw2 = Stopwatch.StartNew();

            // Extract features for all images
            foreach (ListViewItem item in listView1.Items)
            {
                // Get item image
                Bitmap image = originalImages[item.ImageKey] as Bitmap;

                // Process image
                double[] featureVector = bow.GetFeatureVector(image);
                string featureString = featureVector.ToString(DefaultArrayFormatProvider.InvariantCulture);

                if (item.SubItems.Count == 2)
                    item.SubItems[1].Text = featureString;
                else item.SubItems.Add(featureString);

                int classLabel = (item.Tag as Tuple<double[], int>).Item2;
                item.Tag = Tuple.Create(featureVector, classLabel);
            }

            sw2.Stop();

            lbStatus.Text = "BoW constructed in " + sw1.Elapsed + "s. Features extracted in " + sw2.Elapsed + "s.";
            btnSampleRunAnalysis.Enabled = true;
        }


        /// <summary>
        ///   Creates the Support Vector Machines that will identify images based on
        ///   their Bag-of-Visual-Words feature vector representation.
        /// </summary>
        /// 
        private void btnCreateVectorMachines_Click(object sender, EventArgs e)
        {
            double[][] inputs;
            int[] outputs;

            getData(out inputs, out outputs);

            int classes = outputs.Distinct().Count();

            var kernel = getKernel();

            // Create the Multi-class Support Vector Machine using the selected Kernel
            ksvm = new MulticlassSupportVectorMachine(inputs[0].Length, kernel, classes);

            // Create the learning algorithm using the machine and the training data
            MulticlassSupportVectorLearning ml = new MulticlassSupportVectorLearning(ksvm, inputs, outputs);

            // Extract training parameters from the interface
            double complexity = (double)numComplexity.Value;
            double tolerance = (double)numTolerance.Value;
            int cacheSize = (int)numCache.Value;
            SelectionStrategy strategy = (SelectionStrategy)cbStrategy.SelectedItem;

            // Configure the learning algorithm
            ml.Algorithm = (svm, classInputs, classOutputs, i, j) =>
            {
                var smo = new SequentialMinimalOptimization(svm, classInputs, classOutputs);
                smo.Complexity = complexity;
                smo.Tolerance = tolerance;
                smo.CacheSize = cacheSize;
                smo.Strategy = strategy;
                if (kernel is Linear) smo.Compact = true;
                return smo;
            };


            lbStatus.Text = "Training the classifiers. This may take a (very) significant amount of time...";
            Application.DoEvents();

            Stopwatch sw = Stopwatch.StartNew();

            // Train the machines. It should take a while.
            double error = ml.Run();

            sw.Stop();

            lbStatus.Text = String.Format(
                "Training complete ({0}ms, {1}er). Click Classify to test the classifiers.",
                sw.ElapsedMilliseconds, error);

            btnClassifyElimination.Enabled = true;

            // Populate the information tab with the machines
            dgvMachines.Rows.Clear();
            int k = 1;
            for (int i = 0; i < classes; i++)
            {
                for (int j = 0; j < i; j++, k++)
                {
                    var machine = ksvm[i, j];

                    int sv = machine.SupportVectors == null ? 0 : machine.SupportVectors.Length;

                    int c = dgvMachines.Rows.Add(k, i + "-vs-" + j, sv, machine.Threshold);
                    dgvMachines.Rows[c].Tag = machine;
                }
            }

            // approximate size in bytes = 
            //   number of support vectors *
            //   number of doubles in a support vector *
            //   size of double
            int bytes = ksvm.SupportVectorUniqueCount * 1024 * sizeof(double);
            float megabytes = bytes / (1024 * 1024);
            lbSize.Text = String.Format("{0} ({1} MB)", ksvm.SupportVectorUniqueCount, megabytes);
        }



        /// <summary>
        ///   This method automatically estimates a good starting point for 
        ///   the Gaussian's sigma parameter using initialization heuristics.
        /// </summary>
        /// 
        private void btnEstimate_Click(object sender, EventArgs e)
        {
            // Extract inputs
            double[][] inputs;
            int[] outputs;
            getData(out inputs, out outputs);

            DoubleRange range;
            Gaussian g = Gaussian.Estimate(inputs, inputs.Length, out range);

            numSigma.Value = (decimal)g.Sigma;
        }

        /// <summary>
        ///   This method automatically estimates a good starting point for
        ///   the complexity parameter (C) of the SVM learning algorithm.
        /// </summary>
        /// 
        private void btnEstimateC_Click(object sender, EventArgs e)
        {
            // Extract inputs
            double[][] inputs;
            int[] outputs;
            getData(out inputs, out outputs);

            IKernel kernel = getKernel();

            numComplexity.Value = (decimal)SequentialMinimalOptimization.EstimateComplexity(kernel, inputs);
        }

        /// <summary>
        ///   Classifies images into one of the possible classes using the Support
        ///   Vector Machines learned in the previous steps. 
        /// </summary>
        /// 
        private void btnClassify_Click(object sender, EventArgs e)
        {
            int trainingHits = 0;
            int trainingMiss = 0;

            int testingHits = 0;
            int testingMiss = 0;

            // For each image group (i.e. flowers, dolphins)
            foreach (ListViewGroup group in listView1.Groups)
            {
                // For each image item in the group
                foreach (ListViewItem item in group.Items)
                {
                    var info = item.Tag as Tuple<double[], int>;
                    double[] input = info.Item1;
                    int expected = info.Item2;

                    // Classify into one of the classes
                    int actual = ksvm.Compute(input);

                    // Check if we did a correct classification
                    if (expected == actual)
                    {
                        // Yes, we did! Change color to green
                        item.BackColor = Color.LightGreen;
                        if (item.Group.Name.EndsWith(".train"))
                            trainingHits++;
                        else testingHits++;
                    }
                    else
                    {
                        // No, we didn't :( change to red
                        item.BackColor = Color.Firebrick;
                        if (item.Group.Name.EndsWith(".train"))
                            trainingMiss++;
                        else testingMiss++;
                    }
                }
            }

            int trainingTotal = trainingHits + trainingMiss;
            int testingTotal = testingHits + testingMiss;
            lbStatus.Text = String.Format("Classification complete. " +
               "Training: {0}/{1} ({2:00.00}%) hits. Testing: {3}/{4} ({5:00.00}%) hits.",
               trainingHits, trainingTotal, 100 * trainingHits / (double)(trainingTotal),
               testingHits, testingTotal, 100 * testingHits / (double)(testingTotal));
        }





        private void getData(out double[][] inputs, out int[] outputs)
        {
            List<double[]> inputList = new List<double[]>();
            List<int> outputList = new List<int>();

            foreach (ListViewGroup group in listView1.Groups)
            {
                if (group.Name.EndsWith(".train"))
                {
                    foreach (ListViewItem item in group.Items)
                    {
                        var info = item.Tag as Tuple<double[], int>;
                        inputList.Add(info.Item1);
                        outputList.Add(info.Item2);
                    }
                }
            }

            inputs = inputList.ToArray();
            outputs = outputList.ToArray();
        }

        private IKernel getKernel()
        {
            if (rbGaussian.Checked)
            {
                return new Gaussian((double)numSigma.Value);
            }
            else if (rbPolynomial.Checked)
            {
                if (numDegree.Value == 1)
                    return new Linear((double)numConstant.Value);
                else
                    return new Polynomial((int)numDegree.Value, (double)numConstant.Value);
            }
            else if (rbChiSquare.Checked)
            {
                return new ChiSquare();
            }
            else if (rbHistogram.Checked)
            {
                return new HistogramIntersection(1, 1);
            }

            throw new Exception();
        }

       

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            ImageBox.Show(displayImages[listView1.SelectedItems[0].ImageKey]);
        }

        public static IEnumerable<FileInfo> GetFilesByExtensions(DirectoryInfo dir, params string[] extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException("extensions");
            IEnumerable<FileInfo> files = dir.EnumerateFiles();
            return files.Where(f => extensions.Contains(f.Extension));
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
