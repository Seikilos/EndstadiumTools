using System.Collections.Generic;
using CommandLine;

namespace Daemonizer
{
    public class Options
    {
        [Option('e', "exe-path", Required = true,
            HelpText = "Path to the executable to run.")]
        public string ExePath { get; set; }

        [Option('a', "arguments", Required = false, Default = "",
            HelpText = "Arguments to pass to the executable.")]
        public string Arguments { get; set; }

        [Option('l', "log-directory", Required = true,
            HelpText = "Directory where log files will be written.")]
        public string LogDirectory { get; set; }

        [Option('w', "write-always", Required = false, Default = false,
            HelpText = "If set, always write log files even on success.")]
        public bool WriteAlways { get; set; }

        [Option('p', "processes", Required = false, Separator = ',',
            HelpText = "Comma-separated list of process names to monitor and close before running (e.g., firefox,OUTLOOK). If not specified, no processes are monitored.")]
        public IEnumerable<string> Processes { get; set; }
    }
}
