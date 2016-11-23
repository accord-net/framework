using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Accord.Controls
{
    internal static class Tools
    {
        static readonly bool IsRunningOnMono = (Type.GetType("Mono.Runtime") != null);


        public static void ConfigureWindowsFormsApplication()
        {
            if (!IsRunningOnMono && Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}
