using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Daemonizer.Properties;


namespace Daemonizer
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Expect args:
            // Executable arguments event log_file_on_error


            var args = Environment.GetCommandLineArgs().ToList();

            if (args.Count != 4)
            {
                MessageBox.Show(
                    "Daemonzier.exe is a tool allowing to run processes in daemon mode, monitor process and write error log on failure while notifying the user.\n" +
                    "Amount of args given: "+(args.Count-1)+Environment.NewLine +
                    "Args: daemonizer.exe <path to executable> <arguments> <directory for log files>", Resources.Daemonizer_Program_Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(1);
            }

            try
            {
                var exePath = args[1];
                var passedArguments = args[2];
                var logLocation = args[3];

                if (File.Exists(exePath) == false)
                {
                    throw new FileNotFoundException("Could not locate file", exePath);
                }

                if (Directory.Exists(logLocation) == false)
                {
                    throw new DirectoryNotFoundException("Directory not found: "+logLocation);
                }

                var pi = new ProcessStartInfo
                    {
                        FileName = exePath, 
                        Arguments = passedArguments,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false
                    };
                var p = Process.Start(pi);
                p.WaitForExit();
                if (p.ExitCode != 0)
                {
                    // Write std io to log location
                    var filename = string.Format("{0}_{1}.txt", Path.GetFileName(exePath), DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss"));
                    var fullPath = Path.Combine(logLocation, filename);
                    var header = string.Format("Executed: {0} with {1}", exePath, passedArguments);
                    File.WriteAllText(fullPath, header+Environment.NewLine+ p.StandardOutput.ReadToEnd());

                    throw new Exception(string.Format("Execution of {0} failed with exit code {1}. See log file '{2}' for more information.", exePath, p.ExitCode, fullPath));
                }
                

            }
            catch (Exception e)
            {
                MessageBox.Show("Error occurred: " + e, Resources.Daemonizer_Program_Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

    }
}
