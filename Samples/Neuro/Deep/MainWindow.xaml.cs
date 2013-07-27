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

namespace DeepLearning
{
    using System;
    using System.Windows;
    using System.Windows.Forms.DataVisualization.Charting;
    using DeepLearning.ViewModel;
    using MahApps.Metro.Controls;


    /// <summary>
    ///   Main Window View.
    /// </summary>
    /// 
    public partial class MainWindow : MetroWindow
    {

        private MainViewModel viewModel = new MainViewModel();


        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.Learn.ShowDetailRequested += new EventHandler(Learn_ShowDetailRequested);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.Learn.OpenDatabase();

            Chart Chart1 = new Chart();
            ChartArea area = new ChartArea();
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisY.MajorGrid.Enabled = false;
            area.AxisY.IsStartedFromZero = false;

            Chart1.ChartAreas.Add(area);
            Chart1.Series.Add(viewModel.Learn.ErrorPoints);
            windowsFormsHost1.Child = Chart1;
        }



        private void btnAddNewLayer_Click(object sender, RoutedEventArgs e)
        {
            viewModel.StackNewLayer();
            networkDiagram1.Network = viewModel.Network;
        }

        private void btnLearnStart_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Learn.Start();
        }

        private void btnLearnPause_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Learn.Pause();
        }

        private void btnLearnReset_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Learn.Reset();
        }

        private void btnProcessRun_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Learn.Compute();
        }

        private void btnDrawingCanvas_Clear(object sender, RoutedEventArgs e)
        {
            drawingCanvas1.Clear();
        }


        private void button2_Click(object sender, RoutedEventArgs e)
        {
            viewModel.RemoveLastLayer();
            networkDiagram1.Network = viewModel.Network;
        }

        private void btnDream_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Dream.Start();
        }

        private void btnRandomize_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Dream.Randomize();
        }

        private void btnStopDream_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Dream.Stop();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog()
            {
                FileName = "Network",
                DefaultExt = ".ann",
                Filter = "Accord Neural Networks|*.ann"
            };

            if (dlg.ShowDialog().Value)
                viewModel.Load(dlg.FileName);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog()
            {
                FileName = "Network",
                DefaultExt = ".ann",
                Filter = "Accord Neural Networks|*.ann| All files|*.*"
            };

            if (dlg.ShowDialog().Value)
                viewModel.Save(dlg.FileName);
        }

        private void btnComputeDiscover_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Discover.Compute();
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Learn.Detail();
        }

        void Learn_ShowDetailRequested(object sender, EventArgs e)
        {
            PerfViewModel perf = new PerfViewModel(viewModel.Learn.ConfusionMatrix);

            new PerfWindow(perf).Show();
        }
    }
}
