using System;
using System.IO;
using System.Text;

namespace Accord.Setup.Scripts
{
    public static class UpdateVersion
    {
        public static string GetVersionText(string major, string minor, string rev, string build, string tag, bool cpp)
        {
            var sb = new StringBuilder();

            if (cpp)
            {
                sb.AppendLine("#include \"stdafx.h\"");
                sb.AppendLine("using namespace System::Reflection;");
            }
            else
            {
                sb.AppendLine("using System.Reflection;");
            }

            sb.AppendLine("[assembly: AssemblyProductAttribute(\"Accord.NET Framework\")]");
            sb.AppendLine(String.Format("[assembly: AssemblyCopyrightAttribute(\"Copyright (c) Accord.NET authors, 2009-{0}\")]", DateTime.Now.Year));
            sb.AppendLine("[assembly: AssemblyCompanyAttribute(\"Accord.NET\")]");
            sb.AppendLine("[assembly: AssemblyTrademarkAttribute(\"\")]");
            sb.AppendLine("[assembly: AssemblyCultureAttribute(\"\")]");
            sb.AppendLine(String.Format("[assembly: AssemblyVersionAttribute(\"{0}.{1}.{2}\")]", major, minor, rev));
            sb.AppendLine(String.Format("[assembly: AssemblyInformationalVersionAttribute(\"{0}.{1}.{2}-{3}\")]", major, minor, rev, tag));
            sb.AppendLine(String.Format("[assembly: AssemblyFileVersionAttribute(\"{0}.{1}.{2}.{3}\")]", major, minor, rev, build));
            return sb.ToString();
        }

        public static void Replace(string frameworkRootPath)
        {
            var dir = new DirectoryInfo(Path.Combine(frameworkRootPath, "Sources"));
            var files = dir.GetFiles("VersionInfo.*", SearchOption.AllDirectories);

            string version = File.ReadAllText(Path.Combine(frameworkRootPath, "Version.txt"));
            string[] parts;
            parts = version.Split('-');
            string tag = parts[1];
            parts = parts[0].Split('.');
            string major = parts[0];
            string minor = parts[1];
            string rev = parts[2];
            string build = ((int)(DateTime.UtcNow - new DateTime(2001, 1, 1)).TotalDays).ToString();

            Console.WriteLine("Updating source files with version number {0}.{1}.{2}.{3}-{4}", major, minor, rev, build, tag);

            foreach (FileInfo file in files)
            {
                bool cpp = file.Extension == ".cpp";
                string contents = GetVersionText(major, minor, rev, build, tag, cpp);
                File.WriteAllText(file.FullName, contents);
            }
        }

    }
}
