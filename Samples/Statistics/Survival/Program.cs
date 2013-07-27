using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Accord.Statistics.Models.Regression.Linear;
using Accord.Statistics.Models.Regression;
using System.Windows.Forms;

namespace Survival
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

        }
    }
}
