using System;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace Accord.DocFx
{
    class Program
    {

        static void Main(string[] args)
        {
            Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, @"..\..");
            string docfx = Path.GetFullPath(@"..\..\..\..\Sources\packages\docfx.console.2.25.1\build\..\tools\docfx.exe");

            Console.WriteLine("Working dir: " + Environment.CurrentDirectory);
            Console.WriteLine("DocFx path : " + docfx);

            Console.ReadKey();

            Process.Start(@"..\..\..\Sources\packages\docfx.console.2.25.1\build\..\tools\docfx.exe", 
                "--serve").WaitForExit();
        }
    }
}
