using System;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Accord.DocFx
{
    class Program
    {

        static void Main(string[] args)
        {
            Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, @"..\..");
            string docfx = Path.GetFullPath(@"..\..\..\Sources\packages\docfx.console.2.25.1\build\..\tools\docfx.exe");

            Console.WriteLine("Working dir: " + Environment.CurrentDirectory);
            Console.WriteLine("DocFx path : " + docfx);

            Thread.Sleep(5000);

            var p = new Process()
            {
                StartInfo = new ProcessStartInfo(docfx, "--serve")
                {
                    UseShellExecute = false
                }
            };

            p.Start();
            p.WaitForExit();

        }
    }
}
