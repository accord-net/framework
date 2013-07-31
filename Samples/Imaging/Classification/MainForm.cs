// Accord.NET Sample Applications
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
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

using Accord.Controls;
using Accord.Imaging;
using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Kernels;
using AForge;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Classification
{
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


        private void MainForm_Load(object sender, EventArgs e)
        {
            // Seed the number generator with a fixed number so results
            // are replicable accross application runs. Comment the 
            // following line for truly random data splits.
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


        private void btnSampleRunAnalysis_Click(object sender, EventArgs e)
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

        private void btnEstimateC_Click(object sender, EventArgs e)
        {
            // Extract inputs
            double[][] inputs;
            int[] outputs;
            getData(out inputs, out outputs);

            IKernel kernel = getKernel();

            numComplexity.Value = (decimal)SequentialMinimalOptimization.EstimateComplexity(kernel, inputs);
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            ImageBox.Show(displayImages[listView1.SelectedItems[0].ImageKey]);
        }

        private void btnClassifyElimination_Click(object sender, EventArgs e)
        {
            int trainingHits = 0;
            int trainingMiss = 0;

            int testingHits = 0;
            int testingMiss = 0;

            foreach (ListViewGroup group in listView1.Groups)
            {
                foreach (ListViewItem item in group.Items)
                {
                    var info = item.Tag as Tuple<double[], int>;
                    double[] input = info.Item1;
                    int expected = info.Item2;

                    int actual = ksvm.Compute(input);

                    if (expected == actual)
                    {
                        item.BackColor = Color.LightGreen;
                        if (item.Group.Name.EndsWith(".train"))
                            trainingHits++;
                        else testingHits++;
                    }
                    else
                    {
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

        public static IEnumerable<FileInfo> GetFilesByExtensions(DirectoryInfo dir, params string[] extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException("extensions");
            IEnumerable<FileInfo> files = dir.EnumerateFiles();
            return files.Where(f => extensions.Contains(f.Extension));
        }
    }
}
