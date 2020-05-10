using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSBuildRunnerGUI.Contracts;

namespace MSBuildRunnerGUI.Logic
{
    public class FileIO : IFileIO
    {
        public string ReadFile(string file)
        {
            return File.ReadAllText(file);
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public void WriteAllText(string file, string text)
        {
            File.WriteAllText(file, text);
        }

        public int RunProcess(string file, string arguments = null)
        {
            using (var process = new Process())
            {
                // Configure the process using the StartInfo properties.
                process.StartInfo.FileName = file;
               
                if (arguments != null)
                {
                    process.StartInfo.Arguments = arguments;
                }
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                process.Start();
                process.WaitForExit(); // Waits here for the process to exit.

                return process.ExitCode;
            }
        }
    }
}
