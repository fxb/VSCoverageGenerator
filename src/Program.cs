using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.Coverage.Analysis;

using NDesk.Options;

namespace CodeCoverage
{
    class Program
    {
        static void Help(List<string> versions, OptionSet options, string error)
        {
            if (error != null)
            {
                Console.Write("ERROR: ");
                Console.WriteLine(error);
                Console.WriteLine();
            }

            Console.WriteLine("Usage: VSCoverageGenerator.exe <options> <executable> <arguments>");
            Console.WriteLine("Generate a code coverage report for a given executable.");
            Console.WriteLine();

            Console.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();

            if (versions.Count > 0)
            {
                Console.WriteLine("Performance Tools:");
                foreach (string version in versions)
                {
                    Console.WriteLine("  " + version);
                }
                Console.WriteLine();

                Console.WriteLine("Examples:");
                Console.WriteLine("  VSCoverageGenerator.exe TestApp.exe");
                Console.WriteLine("  VSCoverageGenerator.exe -t \"" + versions.Last() + "\" TestApp.exe");
                Console.WriteLine("  VSCoverageGenerator.exe -t \"" + versions.Last() + "\" -e std::* -e boost::* TestApp.exe");
                Console.WriteLine();
            }
        }

        static void Main(string[] args)
        {
            List<string> versions = VisualStudioTools.GetAvailableVersions();
            List<string> excludes = new List<string>();
            List<string> extra = null;
            string version = null;
            string output = null;
            bool help = false;

            var options = new OptionSet()
            {
                { "t|tools=", "Performace Tools version to use.", v => version = v },
                { "e|exclude=", "Exclude symbols from coverage.", v => excludes.Add(v) },
                { "o|output=", "Output directory for coverage report (Default: 'coverage').", v => output = v },
                { "h|help", "Show this help message and exit.", v => help = v != null },
            };

            try
            {
                extra = options.Parse(args);
            }
            catch (OptionException)
            {
                Help(versions, options, null);
                return;
            }

            if (help || versions.Count == 0 || extra.Count == 0)
            {
                Help(versions, options,
                    help ? null :
                    (versions.Count == 0) ? "No Visual Studio Performance Tools found!" :
                    "No executable file given!");
                return;
            }

            version = version ?? versions.Last();

            if (!versions.Contains(version))
            {
                Help(versions, options, "Performance Tools version '" + version + "' not found!");
                return;
            }


            string executableFile = Path.GetFullPath(extra.First());
            string executableName = Path.GetFileName(executableFile);
            string executableDirectory = Path.GetDirectoryName(executableFile);
            string outputDirectory = Path.GetFullPath(
                output ?? Path.Combine(executableDirectory, "coverage"));

            try
            {
                Directory.CreateDirectory(outputDirectory);
            }
            catch (DirectoryNotFoundException ex)
            {
                Help(versions, options, "Couldn't create output directory! " + ex.Message);
                return;
            }

            List<string> arguments = new List<string>();
            arguments.Add("/COVERAGE");
            arguments.Add(executableFile);
            arguments.Add("/OUTPUTPATH:" + outputDirectory);
            arguments.AddRange(excludes.Select(x => "/EXCLUDE:" + x));

            Console.WriteLine("Performing Post-Link Instrumentation...");
            VisualStudioTools.RunPerformanceTool(version, "VSInstr.exe", arguments.ToArray());

            string instrumentedExecutableFile = Path.Combine(outputDirectory, executableName);
            string coverageFile = Path.Combine(outputDirectory, executableName + ".coverage");
            string xmlFile = Path.Combine(outputDirectory, executableName + ".xml");

            Console.WriteLine("Starting Collection...");
            VisualStudioTools.RunPerformanceTool(version, "VSPerfCmd.exe", "/START:COVERAGE", "/OUTPUT:" + coverageFile);

            Console.WriteLine("Running Executable...");
            VisualStudioTools.RunProcess(instrumentedExecutableFile, extra.Skip(1).ToArray());

            Console.WriteLine("Stopping Collection...");
            VisualStudioTools.RunPerformanceTool(version, "VSPerfCmd.exe", "/SHUTDOWN");

            CoverageInfo info = CoverageInfo.CreateFromFile(coverageFile);
            CoverageDS data = info.BuildDataSet();
            data.WriteXml(xmlFile);
        }
    }
}
