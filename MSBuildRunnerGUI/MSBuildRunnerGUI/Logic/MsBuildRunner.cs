using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using MSBuildRunnerGUI.Annotations;

namespace MSBuildRunnerGUI.Logic
{
    public class MsBuildRunner
    {
        public string ExePath { get; private set; }
        public string CommandLine { get; private set; }

        public MsBuildRunner([NotNull] string exePath, [NotNull] string commandLine)
        {
            ExePath = exePath ?? throw new ArgumentNullException(nameof(exePath));
            CommandLine = commandLine ?? throw new ArgumentNullException(nameof(commandLine));

            if (File.Exists(ExePath) == false)
            {
                throw new FileNotFoundException(ExePath);
            }
        }


        public int RunMsBuild(string pathToProjectFile, bool waitForWindow)
        {
            // Dump process call to batch file
            var file = Path.Combine(Path.GetTempPath(), "msbuildrunner.bat");
            var str = $"@echo {file}\r\n\"{ExePath}\" {MakeArguments(pathToProjectFile)}\r\n\r\n{(waitForWindow?"@pause":"")}";
            File.WriteAllText(file, str);

            using (var process = new Process())
            {
                // Configure the process using the StartInfo properties.
                process.StartInfo.FileName = file;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                process.Start();
                process.WaitForExit(); // Waits here for the process to exit.

                return process.ExitCode;
            }

        }

        private string MakeArguments(string pathToProjectFile)
        {
            return CommandLine.Replace("%file%", $"\"{pathToProjectFile}\"");
        }
    }
}
