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
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.DebuggerVisualizers;

[assembly: System.Diagnostics.DebuggerVisualizer(
    typeof( AForge.DebuggerVisualizers.UnmanagedImageVisualizer ),
    typeof( AForge.DebuggerVisualizers.UnmanagedImageObjectSource ),
    Target = typeof( AForge.Imaging.UnmanagedImage ),
    Description = "Unmanaged Image Visualizer" )]

namespace AForge.DebuggerVisualizers
{
    public class UnmanagedImageVisualizer : DialogDebuggerVisualizer
    {
        override protected void Show( IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider )
        {
            Image image = (Image) objectProvider.GetObject( );

            ImageView imageViewer = new ImageView( );
            imageViewer.SetImage( image );

            windowService.ShowDialog( imageViewer );
        }
    }

    public class UnmanagedImageObjectSource : VisualizerObjectSource
    {
        public override void GetData( object target, Stream outgoingData )
        {
            BinaryFormatter bf = new BinaryFormatter( );
            bf.Serialize( outgoingData, ( (AForge.Imaging.UnmanagedImage) target ).ToManagedImage( ) );
        }
    }
}
