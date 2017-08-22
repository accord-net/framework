// Accord Debugging Visualizers
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Darren Schroeder, 2017
// https://github.com/fdncred
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// AForge debugging visualizers
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2011
// contacts@aforgenet.com
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//

using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.DebuggerVisualizers;

[assembly: System.Diagnostics.DebuggerVisualizer(
    typeof(Accord.DebuggerVisualizers.BitmapDataVisualizer),
    typeof(Accord.DebuggerVisualizers.BitmapDataObjectSource),
    Target = typeof(System.Drawing.Imaging.BitmapData),
    Description = "Accord Bitmap Data Visualizer")]

namespace Accord.DebuggerVisualizers
{
    class BitmapDataVisualizer : DialogDebuggerVisualizer
    {
        override protected void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            Image image = (Image)objectProvider.GetObject();

            ImageView imageViewer = new ImageView(image);

            windowService.ShowDialog(imageViewer);
        }
    }

    public class BitmapDataObjectSource : VisualizerObjectSource
    {
        public override void GetData(object target, Stream outgoingData)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(outgoingData,
                (new Accord.Imaging.UnmanagedImage((BitmapData)target)).ToManagedImage());
        }
    }
}
