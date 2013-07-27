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
    using System.ComponentModel;
    using Accord.Controls;
    using Accord.Statistics.Analysis;

    /// <summary>
    ///   View-Model for the Performance window.
    /// </summary>
    /// 
    public class PerfViewModel : INotifyPropertyChanged
    {

        public GeneralConfusionMatrix ConfusionMatrix { get; set; }

        public ConfusionMatrixView ConfusionView { get; private set; }


        public PerfViewModel(GeneralConfusionMatrix matrix)
        {
            ConfusionMatrix = matrix;
            ConfusionView = new ConfusionMatrixView(matrix);
        }

        public PerfViewModel()
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                ConfusionMatrix = new GeneralConfusionMatrix(10, new int[100], new int[100]);
                ConfusionView = new ConfusionMatrixView(ConfusionMatrix);
            }
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
