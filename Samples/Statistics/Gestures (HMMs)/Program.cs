using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Gestures.HMMs
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            Application.Run(new MainForm());
        }
    }
}
