using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CodeCoverage
{
    class VisualStudioTools
    {
        private static Dictionary<string, string> Versions = new Dictionary<string, string>()
        {
            { "Visual Studio 2010", "VS100COMNTOOLS" },
            { "Visual Studio 2012", "VS110COMNTOOLS" },
            { "Visual Studio 2013", "VS120COMNTOOLS" }
        };

        private static string GetPerformanceToolsPath(string version)
        {
            if (!Versions.ContainsKey(version))
            {
                return null;
            }

            string name = Versions[version];
            string path = Environment.GetEnvironmentVariable(name);

            if (path == null)
            {
                return null;
            }

            path = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(path),
                "..", "..", "Team Tools", "Performance Tools"));

            return Directory.Exists(path) ? path : null;
        }

        public static List<string> GetAvailableVersions()
        {
            return Versions.Where(x => GetPerformanceToolsPath(x.Key) != null).Select(x => x.Key).ToList();
        }

        public static void RunProcess(string filename, params string[] arguments)
        {
            Process process = new Process();
            process.StartInfo.FileName = filename;
            process.StartInfo.Arguments = arguments.DefaultIfEmpty("").Aggregate((x, y) => x + " " + y);
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();
        }

        public static void RunPerformanceTool(string version, string filename, params string[] arguments)
        {
            RunProcess(Path.Combine(GetPerformanceToolsPath(version), filename), arguments);
        }
    }
}
