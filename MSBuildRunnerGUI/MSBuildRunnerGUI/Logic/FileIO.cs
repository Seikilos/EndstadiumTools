using System;
using System.Collections.Generic;
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
    }
}
