using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
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

    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            // Expect args:
            // Executable arguments event log_file_on_error


            var args = Environment.GetCommandLineArgs().ToList();

            if (args.Count != 4 && args.Count != 5)
            {
                MessageBox.Show(
                    "Daemonzier.exe is a tool allowing to run processes in daemon mode, monitor process and write error log on failure while notifying the user.\n" +
                    "Amount of args given: " + (args.Count - 1) + Environment.NewLine +
                    "Args: daemonizer.exe <path to executable> <arguments> <directory for log files> <write_always_log>",
                    Resources.Daemonizer_Program_Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(1);
            }

            Process importantProcess = null;
            string modulePath = null;
            DialogResult dlgResult = DialogResult.Ignore;

            int exitCode = 0;

            try
            {
                var exePath = args[1];
                var passedArguments = args[2];
                var logLocation = args[3];
                var writeAlways = args.Count > 4 && bool.Parse(args[4]);

                if (File.Exists(exePath) == false)
                {
                    throw new FileNotFoundException("Could not locate file", exePath);
                }

                if (Directory.Exists(logLocation) == false)
                {
                    throw new DirectoryNotFoundException("Directory not found: " + logLocation);
                }


                importantProcess = Process.GetProcessesByName("firefox").FirstOrDefault();

                if (importantProcess != null)
                {
                    // Find the most parent process of firefox
                    Process parent = importantProcess.Parent();
                    while (parent.ProcessName == "firefox")
                    {
                        importantProcess = parent;
                        try
                        {
                            parent = importantProcess.Parent();
                        }
                        catch (Exception)
                        {
                            break;
                        }
                       
                    }


                    modulePath = importantProcess.MainModule.FileName;
                    dlgResult = MessageBox.Show(null,
                        $"Found {importantProcess.ProcessName}, which may prevent backup to properly finish. Should it be closed and reopened automatically later?" +
                        $"{Environment.NewLine}" +
                        "Yes: Close and reopen | No: Continue"
                        , "Programm detected", MessageBoxButtons.YesNoCancel);

                    if (dlgResult == DialogResult.Yes)
                    {
                        importantProcess.WaitForInputIdle(10000);
                        bool res = importantProcess.CloseMainWindow();

                        if (res)
                        {
                            importantProcess.WaitForExit(30000);
                        }


                        if (importantProcess.HasExited == false)
                        {
                            dlgResult = DialogResult.Ignore; // Prevents restart of process


                            // Waited but process hang, raise error
                            throw new InvalidOperationException(
                                $"Waited for process {importantProcess.ProcessName} to exit but it timed out, aborting");
                        }
                    }

                    if (dlgResult == DialogResult.Cancel)
                    {
                        Environment.Exit(2);
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
                if (importantProcess != null && dlgResult == DialogResult.Yes)
                {
                    Process.Start(modulePath);
                }
            }

            Environment.Exit(exitCode);
        }
    }
}