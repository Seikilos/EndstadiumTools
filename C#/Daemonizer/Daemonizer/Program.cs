using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CommandLine;
using CommandLine.Text;
using Daemonizer.Properties;

namespace Daemonizer
{
    public static class ProcessExtensions
    {
        private static string FindIndexedProcessName(int pid)
        {
            var processName = Process.GetProcessById(pid).ProcessName;
            var processesByName = Process.GetProcessesByName(processName);
            string processIndexdName = null;

            for (var index = 0; index < processesByName.Length; index++)
            {
                processIndexdName = index == 0 ? processName : processName + "#" + index;
                var processId = new PerformanceCounter("Process", "ID Process", processIndexdName);
                if ((int) processId.NextValue() == pid)
                {
                    return processIndexdName;
                }
            }

            return processIndexdName;
        }

        private static Process FindPidFromIndexedProcessName(string indexedProcessName)
        {
            var parentId = new PerformanceCounter("Process", "Creating Process ID", indexedProcessName);
            return Process.GetProcessById((int) parentId.NextValue());
        }

        public static Process Parent(this Process process)
        {
            return FindPidFromIndexedProcessName(FindIndexedProcessName(process.Id));
        }
    }

    /// <summary>
    /// Stores information about a monitored process
    /// </summary>
    internal class ProcessInfo
    {
        public Process Process { get; set; }
        public string ModulePath { get; set; }
        public string ProcessName { get; set; }
        public bool WasClosed { get; set; }
    }

    internal static class Program
    {
        /// <summary>
        /// Finds all monitored processes and returns their parent-most instances
        /// </summary>
        private static List<ProcessInfo> FindMonitoredProcesses(string[] processNames)
        {
            var hashset = new HashSet<string>(processNames.Select(s => s.ToLower()));

            var result = new List<ProcessInfo>();

            var processesLower = Process.GetProcesses().Where(p => hashset.Contains(p.ProcessName.ToLower())).ToList();

            foreach (var p in processesLower)
            {
                result.Add(new ProcessInfo
                {
                    Process = p,
                    ModulePath = p.MainModule.FileName,
                    ProcessName = p.ProcessName,
                    WasClosed = false
                });
            }

            return result;
        }

        private static void Main()
        {
            var result = Parser.Default.ParseArguments<Options>(Environment.GetCommandLineArgs().Skip(1));

            result.WithParsed(opts => RunWithOptions(opts))
                  .WithNotParsed(errors => HandleParseErrors(result, errors));
        }

        private static void HandleParseErrors(ParserResult<Options> result, IEnumerable<Error> errors)
        {
            // CommandLineParser generates complete help text with errors included
            var helpText = HelpText.AutoBuild(result);

            // Check if this is a help/version request (not an actual error)
            var isHelpRequest = errors.Any(e => e.Tag == ErrorType.HelpRequestedError || e.Tag == ErrorType.VersionRequestedError);
            var icon = isHelpRequest ? MessageBoxIcon.Information : MessageBoxIcon.Error;

            MessageBox.Show(helpText.ToString(), Resources.Daemonizer_Program_Name, MessageBoxButtons.OK, icon);
            Environment.Exit(isHelpRequest ? 0 : 1);
        }

        private static void RunWithOptions(Options opts)
        {
            // Get processes to monitor from options (empty array if not specified)
            var processNamesToMonitor = opts.Processes?.ToArray() ?? new string[0];

            var monitoredProcesses = new List<ProcessInfo>();
            var originalProcessNames = new List<string>();
            DialogResult dlgResult = DialogResult.Ignore;

            int exitCode = 0;

            try
            {
                var exePath = opts.ExePath;
                var passedArguments = opts.Arguments;
                var logLocation = opts.LogDirectory;
                var writeAlways = opts.WriteAlways;

                if (File.Exists(exePath) == false)
                {
                    throw new FileNotFoundException("Could not locate file", exePath);
                }

                if (Directory.Exists(logLocation) == false)
                {
                    throw new DirectoryNotFoundException("Directory not found: " + logLocation);
                }

                // Find all monitored processes
                monitoredProcesses = FindMonitoredProcesses(processNamesToMonitor);

                if (monitoredProcesses.Any())
                {
                    // Store the original process names before they are closed
                    originalProcessNames = monitoredProcesses.Select(p => p.ProcessName).Distinct().ToList();
                    bool allProcessesClosed = false;

                    while (!allProcessesClosed)
                    {
                        // Build a list of process names for the dialog
                        var processListText = string.Join(", ", monitoredProcesses.Select(p => p.ProcessName).Distinct());

                        dlgResult = MessageBox.Show(null,
                            $"Found the following processes which may prevent backup from properly finishing: {processListText}" +
                            $"{Environment.NewLine}{Environment.NewLine}" +
                            "Please close these processes manually." +
                            $"{Environment.NewLine}{Environment.NewLine}" +
                            "A message will appear when you can start them again." +
                            $"{Environment.NewLine}{Environment.NewLine}" +
                            "OK: Continue | Cancel: Abort"
                            , "Processes detected", MessageBoxButtons.OKCancel);

                        if (dlgResult == DialogResult.Cancel)
                        {
                            Environment.Exit(2);
                        }

                        // Check if all monitored processes are actually closed
                        monitoredProcesses = FindMonitoredProcesses(processNamesToMonitor);

                        if (!monitoredProcesses.Any())
                        {
                            allProcessesClosed = true;
                        }
                    }
                }


                var filename = string.Format("{0}_{1}.txt", Path.GetFileName(exePath),
                    DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss"));
                var fullPath = Path.Combine(logLocation, filename);

                using (var p = new Process())
                {
                    p.StartInfo = new ProcessStartInfo
                    {
                        FileName = exePath,
                        Arguments = passedArguments,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false
                    };

                    // Use async read from standard output to prevent puffer overflow and dead lock
                    // create buffer file and dump process return

                    using (var outputWaitHandle = new AutoResetEvent(false))
                    using (var errorWaitHandle = new AutoResetEvent(false))
                    using (var stream = new StreamWriter(fullPath, false)) // use default buffer size
                    {
                        stream.WriteLine("Executed: {0} with {1}", exePath, passedArguments);


                        // Register notifiers
                        p.OutputDataReceived += (sender, e) =>
                        {
                            if (e.Data == null)
                            {
                                outputWaitHandle.Set();
                            }
                            else
                            {
                                stream.WriteLine(e.Data);
                            }
                        };
                        p.ErrorDataReceived += (sender, e) =>
                        {
                            if (e.Data == null)
                            {
                                errorWaitHandle.Set();
                            }
                            else
                            {
                                stream.WriteLine("Error: {0}", e.Data);
                            }
                        };


                        p.Start();

                        //p.WaitForExit(10000);


                        p.BeginOutputReadLine();
                        p.BeginErrorReadLine();


                        // This might dead lock forever. Probably add timeout, but unsure what size
                        p.WaitForExit();
                        outputWaitHandle.WaitOne();
                        errorWaitHandle.WaitOne();


                        if (p.ExitCode != 0)
                        {
                            // Write std io to log location
                            throw new Exception(string.Format(
                                "Execution of {0} failed with exit code {1}. See log file '{2}' for more information.",
                                exePath, p.ExitCode, fullPath));
                        }
                    }
                }

                if (writeAlways == false)
                {
                    // Delete the buffered file
                    File.Delete(fullPath);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error occurred: " + e, Resources.Daemonizer_Program_Name, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                exitCode = 1;
            }
            finally
            {
                // Show message that processes can be restarted
                if (dlgResult == DialogResult.OK && originalProcessNames.Any())
                {
                    var processListText = string.Join(", ", originalProcessNames);

                    MessageBox.Show(null,
                        $"You can now restart the following processes: {processListText}",
                        Resources.Daemonizer_Program_Name,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }

            Environment.Exit(exitCode);
        }
    }
}