using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Linq;

namespace Accord.Setup.Archiver
{
    public static class PackageSamples
    {

        public static void Main(string[] args)
        {
            string binDir = Path.GetFullPath("../bin/");

            if (!Directory.Exists(binDir))
                Directory.CreateDirectory(binDir);

            Environment.CurrentDirectory = binDir;
            var samplesDir = new DirectoryInfo("../../Samples/");
            var outputDir = new DirectoryInfo("samples");

            Console.WriteLine();
            Console.WriteLine("Accord.NET Framework sample applications archive builder");
            Console.WriteLine("=========================================================");
            Console.WriteLine("");
            Console.WriteLine("This C# script file will use .NET's System.IO.Compression classes to ");
            Console.WriteLine("automatically build the compressed archives of the sample applications.");
            Console.WriteLine("");
            Console.WriteLine(" - Current directory: {0}", Environment.CurrentDirectory);
            Console.WriteLine(" - Samples directory: {0}", samplesDir.FullName);
            Console.WriteLine(" - Output directory : {0}", outputDir.FullName);
            Console.WriteLine("");

            if (!outputDir.Exists)
                outputDir.Create();

            FileInfo[] csprojs = samplesDir.GetFiles("*.csproj", SearchOption.AllDirectories);
            foreach (FileInfo csproj in csprojs)
                packCsproj(samplesDir, csproj, outputDir);

            FileInfo[] slns = samplesDir.GetFiles("*.sln", SearchOption.AllDirectories);
            foreach (FileInfo sln in slns)
                packSln(samplesDir, sln, outputDir);
        }

        static string createSolution(string rootDir, FileInfo csproj)
        {
            return createSolution(csproj.Name, csproj.FullName.Replace(rootDir, ""), GetProjectGuid(csproj));
        }

        static string createSolution(string projectName, string projectPath, string projectGuid)
        {
            string[] solutionLines =
            {
                "Microsoft Visual Studio Solution File, Format Version 12.00",
                "# Visual Studio 15",
                "VisualStudioVersion = 15.0.26606.0",
                "MinimumVisualStudioVersion = 10.0.40219.1",
                "Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{0}\", \"{1}\", \"{2}\"",
                "EndProject",
                "Global",
                "    GlobalSection(SolutionConfigurationPlatforms) = preSolution",
                "        Debug|x86 = Debug|x86",
                "        Mono|x86 = Mono|x86",
                "        Release|x86 = Release|x86",
                "	EndGlobalSection",
                "	GlobalSection(ProjectConfigurationPlatforms) = postSolution",
                "		{2}.Debug|x86.ActiveCfg = Debug|x86",
                "		{2}.Debug|x86.Build.0 = Debug|x86",
                "		{2}.Mono|x86.ActiveCfg = Release|x86",
                "		{2}.Release|x86.ActiveCfg = Release|x86",
                "		{2}.Release|x86.Build.0 = Release|x86",
                "    EndGlobalSection",
                "EndGlobal",
                ""
            };

            string solutionText = String.Join(Environment.NewLine, solutionLines);
            return String.Format(solutionText, projectName, projectPath, projectGuid);
        }

        public static string GetProjectGuid(FileInfo csproj)
        {
            foreach (string line in File.ReadLines(csproj.FullName))
                if (line.Contains("<ProjectGuid>"))
                    return line.Replace("<ProjectGuid>", "").Replace("</ProjectGuid>", "").Trim();
            throw new Exception();
        }

        public static void packCsproj(DirectoryInfo samplesDir, FileInfo csproj, DirectoryInfo outputDir)
        {
            /* Get project name */
            string projectName = Path.GetFileNameWithoutExtension(csproj.Name);

            /* Get project path */
            string relativeProjectPath = Path.Combine(csproj.Directory.FullName.Replace(samplesDir.FullName, ""));
            string fullProjectPath = Path.Combine(samplesDir.FullName, relativeProjectPath);

            string archiveName = GetArchiveName(relativeProjectPath);

            Console.WriteLine(" - Processing {0}", archiveName);

            string outputPath = Path.Combine(outputDir.FullName, archiveName);
            FileStream fs = new FileStream(outputPath, FileMode.Create);
            using (ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Create))
            {
                WriteSolutionFile(samplesDir, csproj, zip);
                MoveSampleFiles(csproj.Directory, relativeProjectPath, fullProjectPath, zip);
            }
        }

        public static void packSln(DirectoryInfo samplesDir, FileInfo csproj, DirectoryInfo outputDir)
        {
            /*  Get project name */
            string projectName = Path.GetFileNameWithoutExtension(csproj.Name);

            /* Get project path */
            string relativeProjectPath = Path.Combine(csproj.Directory.FullName.Replace(samplesDir.FullName, ""));
            string fullProjectPath = Path.Combine(samplesDir.FullName, relativeProjectPath);

            if (Path.IsPathRooted(relativeProjectPath))
                return;

            string archiveName = GetArchiveName(relativeProjectPath);

            Console.WriteLine(" - Processing {0}", archiveName);

            string outputPath = Path.Combine(outputDir.FullName, archiveName);
            FileStream fs = new FileStream(outputPath, FileMode.Create);
            using (ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Create))
            {
                MoveSampleFiles(csproj.Directory, "", fullProjectPath, zip);
            }
        }

        private static string GetArchiveName(string relativeProjectPath)
        {
            return "accord-" + relativeProjectPath.ToLowerInvariant()
                            .Replace(' ', '-').Replace(",", "")
                            .Replace('/', '-').Replace('\\', '-')
                            .Replace(".-", "-") + ".zip";
        }

        private static void MoveSampleFiles(DirectoryInfo directory, string relativeProjectPath, string fullProjectPath, ZipArchive zip)
        {
            foreach (FileInfo file in directory.GetFiles("*", SearchOption.AllDirectories))
            {
                if (skip(file))
                    continue;

                string path = file.FullName.Replace(fullProjectPath + "\\", "");
                string expandedPath = Path.Combine(relativeProjectPath, path);

                expandedPath = GetRelativeBinPath(file, path, expandedPath, "bin\\x86\\Release", "Binaries\\x86\\");
                expandedPath = GetRelativeBinPath(file, path, expandedPath, "bin\\x64\\Release", "Binaries\\x64\\");

                ZipArchiveEntry entry = zip.CreateEntry(expandedPath);
                using (var entryStream = entry.Open())
                using (var fileStream = file.Open(FileMode.Open))
                {
                    fileStream.CopyTo(entryStream);
                }
            }
        }

        private static string GetRelativeBinPath(FileInfo file, string path, string expandedPath, string binPath, string outputPath)
        {
            /* move the binaries to a folder at the root of the sample */
            if (file.Directory.FullName.Contains(binPath))
            {
                /* get the directory rooted from the solution file */
                int pos = path.IndexOf(binPath) + binPath.Length;
                string fileRelativeToBin = path.Remove(0, pos + 1);
                expandedPath = Path.Combine(outputPath, fileRelativeToBin);
            }

            return expandedPath;
        }

        private static bool skip(FileInfo file)
        {
            if (file.Name.EndsWith(".pdb"))
                return true;
            if (file.Name.EndsWith("GhostDoc.xml"))
                return true;
            if (file.Name.EndsWith(".svn"))
                return true;
            if (file.Name.EndsWith(".git"))
                return true;
            if (file.Name.EndsWith(".user"))
                return true;
            if (file.Name.EndsWith(".gitignore"))
                return true;
            if (file.Name.EndsWith(".suo"))
                return true;
            if (file.Name.EndsWith(".user"))
                return true;
            if (file.Directory.FullName.Contains(".vs"))
                return true;
            if (file.Directory.FullName.Contains("\\obj\\"))
                return true;
            return false;
        }

        private static void WriteSolutionFile(DirectoryInfo samplesDir, FileInfo csproj, ZipArchive zip)
        {
            string solutionName = Path.ChangeExtension(csproj.Name, "sln");
            ZipArchiveEntry entry = zip.CreateEntry(solutionName);
            using (Stream entryStream = entry.Open())
            using (TextWriter streamWriter = new StreamWriter(entryStream))
            {
                streamWriter.Write(createSolution(samplesDir.FullName, csproj));
            }
        }
    }
}
