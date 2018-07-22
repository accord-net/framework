using System;
using System.Drawing;
using System.Windows.Forms;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Bitmap b = (Bitmap)Image.FromFile(@"..\..\..\..\Unit Tests\Accord.Tests.Imaging\Resources\wiki-flower.png");
            Accord.DebuggerVisualizers.ImageVisualizer.TestShowVisualizer(b, this);
        }
    }
}
