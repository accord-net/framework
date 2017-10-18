using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Accord.Setup.Scripts
{
    public static class UpdateVersion
    {
        public static string GetVersionText(string major, string minor, string rev, string build, string tag, bool cpp)
        {
            var sb = new StringBuilder();
            string semicolon;

            if (cpp)
            {
                semicolon = ";";
                sb.AppendLine("#include \"stdafx.h\"");
                sb.AppendLine("using namespace System::Reflection;");
            }
            else
            {
                semicolon = String.Empty;
                sb.AppendLine("using System.Reflection;");
            }

            sb.AppendLine("[assembly: AssemblyProductAttribute(\"Accord.NET Framework\")]" + semicolon);
            sb.AppendLine(String.Format("[assembly: AssemblyCopyrightAttribute(\"Copyright (c) Accord.NET authors, 2009-{0}\")]" + semicolon, DateTime.Now.Year));
            sb.AppendLine("[assembly: AssemblyCompanyAttribute(\"Accord.NET\")]" + semicolon);
            sb.AppendLine("[assembly: AssemblyTrademarkAttribute(\"\")]" + semicolon);
            sb.AppendLine("[assembly: AssemblyCultureAttribute(\"\")]" + semicolon);
            sb.AppendLine(String.Format("[assembly: AssemblyVersionAttribute(\"{0}.{1}.{2}\")]" + semicolon, major, minor, rev));
            sb.AppendLine(String.Format("[assembly: AssemblyInformationalVersionAttribute(\"{0}.{1}.{2}{3}\")]" + semicolon, major, minor, rev, tag));
            sb.AppendLine(String.Format("[assembly: AssemblyFileVersionAttribute(\"{0}.{1}.{2}.{3}\")]" + semicolon, major, minor, rev, build));
            return sb.ToString();
        }

        public static void Replace(string frameworkRootPath)
        {
            string version = File.ReadAllText(Path.Combine(frameworkRootPath, "Version.txt"));
            string[] parts;
            parts = version.Split('-');
            string tag = parts.Length > 1 ? "-" + parts[1] : String.Empty;
            parts = parts[0].Split('.');
            string major = parts[0];
            string minor = parts[1];
            string rev = parts[2];
            string build = ((int)(DateTime.UtcNow - new DateTime(2001, 1, 1)).TotalDays).ToString();

            Console.WriteLine("Updating source files with version number {0}.{1}.{2}.{3}{4}", major, minor, rev, build, tag);

            replaceAssemblyInfo(frameworkRootPath, tag, major, minor, rev, build);
            replaceNetStandardTargets(frameworkRootPath, tag, major, minor, rev, build);
            replaceDocumentation(frameworkRootPath, major, minor, rev);
            replaceAppVeyor(frameworkRootPath, major, minor, rev);
        }

        private static void replaceDocumentation(string frameworkRootPath, string major, string minor, string rev)
        {
            string pattern = "<HelpFileVersion>.*</HelpFileVersion>";
            string docPath = Path.Combine(frameworkRootPath, "Sources/Accord.Docs/Accord.Docs.SHFB/Accord.Docs.SHFB.shfbproj");
            string contents = File.ReadAllText(docPath);
            string replacement = String.Format("<HelpFileVersion>{0}.{1}.{2}.0</HelpFileVersion>", major, minor, rev);
            contents = Regex.Replace(contents, pattern, replacement);
            File.WriteAllText(docPath, contents);
        }

        private static void replaceAppVeyor(string frameworkRootPath, string major, string minor, string rev)
        {
            string pattern = "version: .*";
            string docPath = Path.Combine(frameworkRootPath, ".appveyor.yml");
            string contents = File.ReadAllText(docPath);
            string replacement = String.Format("version: {0}.{1}.{2}.{{build}}", major, minor, rev);
            contents = Regex.Replace(contents, pattern, replacement);
            File.WriteAllText(docPath, contents);
        }

        private static void replaceAssemblyInfo(string frameworkRootPath, string tag, string major, string minor, string rev, string build)
        {
            var dir = new DirectoryInfo(Path.Combine(frameworkRootPath, "Sources"));
            var files = dir.GetFiles("VersionInfo.*", SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                if (file.Extension != ".cpp" && file.Extension != ".cs")
                    continue;

                bool cpp = file.Extension == ".cpp";
                string contents = GetVersionText(major, minor, rev, build, tag, cpp);
                File.WriteAllText(file.FullName, contents);
            }
        }

        private static void replaceNetStandardTargets(string frameworkRootPath, string tag, string major, string minor, string rev, string build)
        {
            string targetsFile = Path.Combine(frameworkRootPath, "Sources", "Version.targets");

            var sb = new StringBuilder();

            sb.AppendLine("<Project>");
            sb.AppendLine("  <PropertyGroup>");
            sb.AppendLine(string.Format("    <Copyright>Copyright (c) Accord.NET authors, 2009-{0}</Copyright>", DateTime.Now.Year));
            sb.AppendLine("    <Version>3.5.0</Version>");
            sb.AppendLine(string.Format("    <AssemblyVersion>{0}.{1}.{2}</AssemblyVersion>", major, minor, rev));
            sb.AppendLine(string.Format("    <AssemblyInformationalVersion>{0}.{1}.{2}{3}</AssemblyInformationalVersion>", major, minor, rev, tag));
            sb.AppendLine(string.Format("    <FileVersion>{0}.{1}.{2}.{3}</FileVersion>", major, minor, rev, build));
            sb.AppendLine("  </PropertyGroup>");
            sb.AppendLine("</Project>");
            
            File.WriteAllText(targetsFile, sb.ToString());
        }
    }
}
