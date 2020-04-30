using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBuildRunnerGUI.Contracts
{
    public interface IFileIO
    {

        string ReadFile(string file);

        bool Exists(string path);
    }
}
