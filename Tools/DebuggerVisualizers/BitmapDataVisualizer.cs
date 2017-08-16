// AForge debugging visualizers
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2011
// contacts@aforgenet.com
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
