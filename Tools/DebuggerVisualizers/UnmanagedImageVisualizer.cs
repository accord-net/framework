// AForge debugging visualizers
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2011
// contacts@aforgenet.com
//

using System.IO;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.DebuggerVisualizers;

[assembly: System.Diagnostics.DebuggerVisualizer(
    typeof(Accord.DebuggerVisualizers.UnmanagedImageVisualizer),
    typeof(Accord.DebuggerVisualizers.UnmanagedImageObjectSource),
    Target = typeof(Accord.Imaging.UnmanagedImage),
    Description = "Accord Unmanaged Image Visualizer")]

namespace Accord.DebuggerVisualizers
{
    public class UnmanagedImageVisualizer : DialogDebuggerVisualizer
    {
        override protected void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            Image image = (Image)objectProvider.GetObject();

            ImageView imageViewer = new ImageView(image);

            windowService.ShowDialog(imageViewer);
        }
    }

    public class UnmanagedImageObjectSource : VisualizerObjectSource
    {
        public override void GetData(object target, Stream outgoingData)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(outgoingData, ((Accord.Imaging.UnmanagedImage)target).ToManagedImage());
        }
    }
}
