using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using MSBuildRunnerGUI.Annotations;
using MSBuildRunnerGUI.Contracts;

namespace MSBuildRunnerGUI.Logic
{
    public class MsBuildRunner
    {
        private IFileIO _fileIO;

        public string ExePath { get; private set; }
        public string CommandLine { get; private set; }

        public const string FileMarker = "%file%";
        public const string FileNameMarker = "%filename%";

        public static Dictionary<string,string> SupportedMacros { get; } = new Dictionary<string, string>
        {
            {FileMarker, "The project file. Enclose it in quotes if whitespaces are possible." },
            {FileNameMarker, "The project file name without extension. Enclose it in quotes if whitespaces are possible." }
        };


        public MsBuildRunner([NotNull] IFileIO fileIO, [NotNull] string exePath, [NotNull] string commandLine)
        {
            _fileIO = fileIO ?? throw new ArgumentNullException(nameof(fileIO));
            ExePath = exePath ?? throw new ArgumentNullException(nameof(exePath));
            CommandLine = commandLine ?? throw new ArgumentNullException(nameof(commandLine));

            if (_fileIO.Exists(ExePath) == false)
            {
                throw new FileNotFoundException(ExePath);
            }
        }


        public int RunMsBuild(string pathToProjectFile, bool waitForWindow)
        {
            // Dump process call to batch file
            var file = Path.Combine(Path.GetTempPath(), "msbuildrunner.bat");
            var str = $"@echo {file}\r\n\"{ExePath}\" {MakeArguments(pathToProjectFile)}\r\n\r\n{(waitForWindow?"@pause":"")}";
            _fileIO.WriteAllText(file, str);

           return _fileIO.RunProcess(file);

        }

        private string MakeArguments(string pathToProjectFile)
        {
            return CommandLine
                .Replace(FileMarker, $"\"{pathToProjectFile}\"")
                .Replace(FileNameMarker, Path.GetFileNameWithoutExtension(pathToProjectFile));
        }
    }
}
