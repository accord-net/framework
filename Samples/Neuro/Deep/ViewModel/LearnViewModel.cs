// Accord.NET Sample Applications
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2012
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

namespace DeepLearning.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows.Forms.DataVisualization.Charting;
    using System.Windows.Threading;
    using Accord.Math;
    using Accord.Neuro;
    using Accord.Neuro.Learning;
    using Accord.Statistics.Analysis;
    using AForge.Neuro;
    using AForge.Neuro.Learning;
    using DeepLearning.Databases;

    /// <summary>
    ///   View-Model for the Learning tab.
    /// </summary>
    /// 
    public class LearnViewModel : INotifyPropertyChanged
    {
        public MainViewModel Main { get; private set; }

        public ObservableCollection<Sample> Display { get; set; }

        public Set CurrentSet { get; set; }
        public Set[] Sets { get; private set; }

        public GeneralConfusionMatrix ConfusionMatrix { get; private set; }


        /// <summary>Indicates whether a background thread is busy loading data.</summary>
        public bool IsDataLoading { get; set; }

        /// <summary>Indicates whether training data has already finished loading.</summary>
        public bool HasDataLoaded { get; private set; }

        /// <summary>Indicates the learning procedure is starting.</summary>
        public bool IsStarting { get; set; }

        /// <summary>Indicates the learning procedure is currently being run.</summary>
        public bool IsLearning { get; private set; }

        public bool HasLearned { get; private set; }

        public string CurrentTask
        {
            get
            {
                if (ShouldLearnEntireNetwork)
                    return "Fine-tuning entire network";
                return "Learn network layer " + SelectedLayerIndex + " of " + Main.Network.Layers.Length;
            }
        }


        // Training controls
        public bool CanStart
        {
            get
            {
                return (HasDataLoaded && !IsLearning) &&
                    ((!ShouldLearnEntireNetwork && !ShouldLayerBeSupervised && Main.CanGenerate) ||
                    ((ShouldLearnEntireNetwork || ShouldLayerBeSupervised) && Main.CanClassify));
            }
        }

        public bool CanPause { get { return IsLearning; } }
        public bool CanReset { get { return Main.CanGenerate; } }
        public bool CanTest { get { return HasDataLoaded && Main.CanClassify && !IsLearning; } }

        public bool CanLearnUnsupervised { get { return Main.Network != null; } }
        public bool CanLayerBeSupervised
        {
            get
            {
                return CanLearnUnsupervised && !ShouldLearnEntireNetwork &&
                    Main.Network.Layers[SelectedLayerIndex - 1].Neurons.Length == Main.Database.Classes;
            }
        }

        public bool CanNetworkBeSupervised
        {
            get { return Main.CanClassify; }
        }


        // Training parameters
        public int SelectedLayerIndex { get; set; }
        public bool ShouldLayerBeSupervised { get; set; }
        public bool ShouldLearnEntireNetwork { get; set; }

        public double LearningRate { get; set; }
        public double Momentum { get; set; }
        public double WeightDecay { get; set; }
        public int Epochs { get; set; }
        public int BatchSize { get; set; }


        // Training session information
        public int CurrentEpoch { get; private set; }
        public double CurrentError { get; private set; }
        public Series ErrorPoints { get; set; }

        public event EventHandler ShowDetailRequested;

        
        private bool shouldStop;


        public LearnViewModel(MainViewModel main)
        {
            this.Main = main;

            SelectedLayerIndex = 1;

            LearningRate = 0.1;
            WeightDecay = 0.001;
            Momentum = 0.9;
            Epochs = 50;
            BatchSize = 100;

            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                HasLearned = true;

            Sets = new[] { Set.Training, Set.Testing };
            CurrentSet = Set.Testing;

            ErrorPoints = new Series();
            ErrorPoints.ChartType = SeriesChartType.Line;

            this.PropertyChanged += new PropertyChangedEventHandler(LearnViewModel_PropertyChanged);
            Main.PropertyChanged += new PropertyChangedEventHandler(Main_PropertyChanged);
        }

        public LearnViewModel()
        {

        }



        private void Main_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Network")
            {
                PropertyChanged(this, new PropertyChangedEventArgs("CurrentTask"));
                PropertyChanged(this, new PropertyChangedEventArgs("CanNetworkBeSupervised"));
            }
        }

        private void LearnViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentSet") updateDisplay();
            if (e.PropertyName == "CanLayerBeSupervised")
                if (!CanLayerBeSupervised) ShouldLayerBeSupervised = ShouldLearnEntireNetwork;
        }



        private void updateDisplay()
        {
            if (HasDataLoaded)
            {
                ObservableCollection<Sample> dataset = (CurrentSet == Set.Training) ?
                    Main.Database.Training : Main.Database.Testing;
                Display = dataset;
            }
        }


        public void OpenDatabase()
        {
            IsDataLoading = true;

            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

            new Task(() =>
            {
                Main.Database.Load();

                dispatcher.BeginInvoke((Action)(() =>
                {
                    IsDataLoading = false;
                    HasDataLoaded = true;
                    updateDisplay();
                }));

            }).Start();
        }

        public void Start()
        {
            if (!CanStart) return;

            IsLearning = true;
            IsStarting = true;
            shouldStop = false;

            ErrorPoints.Points.Clear();
            CurrentEpoch = 0;
            CurrentError = 0;

            if (ShouldLearnEntireNetwork)
                learnNetworkSupervised();

            else if (ShouldLayerBeSupervised)
                learnLayerSupervised();

            else learnLayerUnsupervised();
        }

        private void learnLayerUnsupervised()
        {
            if (!Main.CanGenerate) return;
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

            new Task(() =>
            {
                DeepBeliefNetworkLearning teacher = new DeepBeliefNetworkLearning(Main.Network)
                {
                    Algorithm = (h, v, i) => new ContrastiveDivergenceLearning(h, v)
                    {
                        LearningRate = LearningRate,
                        Momentum = 0.5,
                        Decay = WeightDecay,
                    },

                    LayerIndex = SelectedLayerIndex - 1,
                };

                double[][] inputs;
                Main.Database.Training.GetInstances(out inputs);
                int batchCount = Math.Max(1, inputs.Length / BatchSize);

                // Create mini-batches to speed learning
                int[] groups = Accord.Statistics.Tools
                    .RandomGroups(inputs.Length, batchCount);
                double[][][] batches = inputs.Subgroups(groups);

                // Gather learning data for the layer
                double[][][] layerData = teacher.GetLayerInput(batches);
                var cd = teacher.GetLayerAlgorithm(teacher.LayerIndex) as ContrastiveDivergenceLearning;

                // Start running the learning procedure
                for (int i = 0; i < Epochs && !shouldStop; i++)
                {
                    double error = teacher.RunEpoch(layerData) / inputs.Length;

                    dispatcher.BeginInvoke((Action<int, double>)updateError,
                        DispatcherPriority.ContextIdle, i + 1, error);

                    if (i == 10)
                        cd.Momentum = Momentum;
                }

                IsLearning = false;

            }).Start();
        }

        private void learnNetworkSupervised()
        {
            if (!Main.CanClassify) return;
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

            new Task(() =>
            {
                var teacher = new BackPropagationLearning(Main.Network)
                {
                    LearningRate = LearningRate,
                    Momentum = Momentum
                };

                double[][] inputs, outputs;
                Main.Database.Training.GetInstances(out inputs, out outputs);

                // Start running the learning procedure
                for (int i = 0; i < Epochs && !shouldStop; i++)
                {
                    double error = teacher.RunEpoch(inputs, outputs);

                    dispatcher.BeginInvoke((Action<int, double>)updateError,
                        DispatcherPriority.ContextIdle, i + 1, error);
                }

                Main.Network.UpdateVisibleWeights();
                IsLearning = false;

            }).Start();
        }

        private void learnLayerSupervised()
        {
            if (!Main.CanClassify) return;
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

            new Task(() =>
            {
                DeepNeuralNetworkLearning teacher = new DeepNeuralNetworkLearning(Main.Network)
                {
                    Algorithm = (ann, i) => new ParallelResilientBackpropagationLearning(ann),
                    LayerIndex = Main.Network.Layers.Length - 1,
                };

                double[][] inputs, outputs;
                Main.Database.Training.GetInstances(out inputs, out outputs);

                // Gather learning data for the layer
                double[][] layerData = teacher.GetLayerInput(inputs);

                // Start running the learning procedure
                for (int i = 0; i < Epochs && !shouldStop; i++)
                {
                    double error = teacher.RunEpoch(layerData, outputs);

                    dispatcher.BeginInvoke((Action<int, double>)updateError,
                        DispatcherPriority.ContextIdle, i + 1, error);
                }

                Main.Network.UpdateVisibleWeights();
                IsLearning = false;

            }).Start();
        }

        private void updateError(int epoch, double error)
        {
            IsStarting = false;
            CurrentEpoch = epoch;
            CurrentError = error;
            ErrorPoints.Points.Add(error);
            HasLearned = true;
        }

        public void Pause()
        {
            shouldStop = true;
        }

        public void Reset()
        {
            if (ShouldLayerBeSupervised)
            {
                new NguyenWidrow(Main.Network.Machines[SelectedLayerIndex - 1])
                 .Randomize();
            }
            else
            {
                new GaussianWeights(Main.Network.Machines[SelectedLayerIndex - 1])
                    .Randomize();
            }

            Main.Network.UpdateVisibleWeights();

            CurrentEpoch = 0;
            CurrentEpoch = 0;
            HasLearned = false;
        }


        public void Compute()
        {
            if (!CanTest) return;

            IsDataLoading = true;

            ObservableCollection<Sample> set = Display;
            Display = null; // remove interface bindings

            ActivationNetwork network = Main.Network;
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

            new Task(() =>
            {
                int[] actual = new int[set.Count];
                int[] expected = new int[set.Count];

                int hits = 0;

                Parallel.For(0, set.Count, i =>
                {
                    double[] input = set[i].Features;
                    double[] output = network.Compute(input);

                    int imax; output.Max(out imax);

                    set[i].Result = imax;

                    actual[i] = imax;
                    expected[i] = set[i].Class;

                    if (set[i].Match.Value)
                        hits++;
                });

                int classCount = Main.Database.Classes;
                var confusionMatrix = new GeneralConfusionMatrix(classCount, expected, actual);

                dispatcher.BeginInvoke((Action)(() =>
                {
                    ConfusionMatrix = confusionMatrix;
                    IsDataLoading = false;
                    Display = set;
                }));

            }).Start();
        }


        public void Detail()
        {
            if (ConfusionMatrix == null) return;

            if (ShowDetailRequested != null)
                ShowDetailRequested(this, EventArgs.Empty);
        }


        // The PropertyChanged event doesn't needs to be explicitly raised
        // from this application. The event raising is handled automatically
        // by the NotifyPropertyWeaver VS extension using IL injection.
        //
#pragma warning disable 0067
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }
}
