using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBuildRunnerGUI.Persistence
{
    public class UserSettings
    {
        public string MsBuildCommandLine { get; set; }

        public List<bool> ActiveStates { get; set; } = new List<bool>();

        public List<int> TokenPositions { get; set; } = new List<int>();

        public string MsBuildExePath { get; set; }

        public string LastSetDirectory { get; set; }
    }
}
